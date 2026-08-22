using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Catalog;
using ECommerce.Email;
using ECommerce.Marketing;
using ECommerce.Payment;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Orders;

/// <summary>
/// Admin: order list (with filters), detail, status update, shipments, refund; inventory deduction on confirmation.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize("ECommerce.Administration")]
public class OrderAdminAppService : ECommerceAppService, IOrderAdminAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<OrderLine, Guid> _orderLineRepository;
    private readonly IRepository<OrderStatusHistory, Guid> _orderStatusHistoryRepository;
    private readonly IRepository<Shipment, Guid> _shipmentRepository;
    private readonly IInventoryDeductionService _inventoryDeduction;
    private readonly ITransactionalEmailService _transactionalEmail;
    private readonly ILoyaltyPointsService _loyaltyPointsService;
    private readonly IPaymentAppService _paymentAppService;

    public OrderAdminAppService(
        IRepository<Order, Guid> orderRepository,
        IRepository<OrderLine, Guid> orderLineRepository,
        IRepository<OrderStatusHistory, Guid> orderStatusHistoryRepository,
        IRepository<Shipment, Guid> shipmentRepository,
        IInventoryDeductionService inventoryDeduction,
        ITransactionalEmailService transactionalEmail,
        ILoyaltyPointsService loyaltyPointsService,
        IPaymentAppService paymentAppService)
    {
        _orderRepository = orderRepository;
        _orderLineRepository = orderLineRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _shipmentRepository = shipmentRepository;
        _inventoryDeduction = inventoryDeduction;
        _transactionalEmail = transactionalEmail;
        _loyaltyPointsService = loyaltyPointsService;
        _paymentAppService = paymentAppService;
    }

    public async Task<PagedResultDto<OrderListDto>> GetListAsync(OrderListRequestDto input)
    {
        var query = await _orderRepository.GetQueryableAsync();
        if (!string.IsNullOrWhiteSpace(input.Status) && Enum.TryParse<OrderStatus>(input.Status, true, out var statusFilter))
            query = query.Where(o => o.Status == statusFilter);
        if (input.DateFrom.HasValue)
            query = query.Where(o => o.CreationTime >= input.DateFrom.Value);
        if (input.DateTo.HasValue)
            query = query.Where(o => o.CreationTime < input.DateTo.Value);
        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            var term = input.Search.Trim().ToLower();
            query = query.Where(o =>
                (o.ContactEmail != null && o.ContactEmail.ToLower().Contains(term)) ||
                (o.ContactName != null && o.ContactName.ToLower().Contains(term)));
        }

        var total = await AsyncExecuter.CountAsync(query);

        var sortDesc = input.Sorting?.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase) ?? true;
        query = sortDesc ? query.OrderByDescending(o => o.CreationTime) : query.OrderBy(o => o.CreationTime);

        var skip = input.SkipCount;
        var take = input.MaxResultCount > 0 ? input.MaxResultCount : 10;
        var orders = await AsyncExecuter.ToListAsync(query.Skip(skip).Take(take));

        var items = orders.Select(o => new OrderListDto
        {
            Id = o.Id,
            Status = o.Status.ToString(),
            PaymentStatus = o.PaymentStatus.ToString(),
            ContactEmail = o.ContactEmail,
            ContactName = o.ContactName,
            Total = o.Total,
            CreationTime = o.CreationTime,
            UserId = o.UserId,
        }).ToList();

        return new PagedResultDto<OrderListDto>(total, items);
    }

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        var lines = await _orderLineRepository.GetListAsync(l => l.OrderId == id);
        var history = await _orderStatusHistoryRepository.GetListAsync(h => h.OrderId == id);
        return MapToDto(order, lines, history.OrderBy(h => h.CreationTime).ToList());
    }

    public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Status))
            throw new Volo.Abp.BusinessException("ECommerce:OrderStatusRequired");

        if (!Enum.TryParse<OrderStatus>(input.Status, true, out var newStatus))
            throw new Volo.Abp.BusinessException("ECommerce:InvalidOrderStatus").WithData("Status", input.Status);

        var order = await _orderRepository.GetAsync(id);
        var previousStatus = order.Status;
        if (previousStatus == newStatus)
            return await GetAsync(id);

        if (newStatus == OrderStatus.Confirmed)
        {
            var lines = await _orderLineRepository.GetListAsync(l => l.OrderId == id);
            var toDeduct = lines.Select(l => (l.ProductVariantId, l.Quantity)).ToList();
            await _inventoryDeduction.DeductForOrderLinesAsync(toDeduct);
            var lineInfos = lines.Select(l => new OrderLineInfo
            {
                ProductName = l.ProductName,
                Sku = l.Sku,
                UnitPrice = l.UnitPrice,
                Quantity = l.Quantity,
                LineTotal = l.LineTotal
            }).ToList();
            await _transactionalEmail.SendOrderConfirmationAsync(order.Id, order.ContactEmail, order.ContactName ?? "", order.Total, lineInfos);
            await _loyaltyPointsService.AwardPointsForOrderAsync(order.Id, order.UserId, order.Total);
        }
        else if (newStatus == OrderStatus.Shipped)
        {
            var trackingInfo = string.IsNullOrWhiteSpace(input.TrackingNumber)
                ? null
                : string.IsNullOrWhiteSpace(input.Carrier)
                    ? input.TrackingNumber
                    : $"{input.Carrier}: {input.TrackingNumber}";
            var shipment = new Shipment(
                GuidGenerator.Create(),
                order.Id,
                input.Carrier,
                input.TrackingNumber,
                DateTime.UtcNow,
                null);
            await _shipmentRepository.InsertAsync(shipment);
            await _transactionalEmail.SendShippingNotificationAsync(order.Id, order.ContactEmail, order.ContactName ?? "", trackingInfo);
        }
        else if (newStatus == OrderStatus.Cancelled && previousStatus >= OrderStatus.Confirmed)
        {
            var lines = await _orderLineRepository.GetListAsync(l => l.OrderId == id);
            var toRestore = lines.Select(l => (l.ProductVariantId, l.Quantity)).ToList();
            await _inventoryDeduction.RestoreForOrderLinesAsync(toRestore);
        }

        order.SetStatus(newStatus);
        await _orderRepository.UpdateAsync(order);

        var historyEntry = new OrderStatusHistory(GuidGenerator.Create(), order.Id, newStatus);
        await _orderStatusHistoryRepository.InsertAsync(historyEntry);

        return await GetAsync(id);
    }

    public async Task<List<ShipmentDto>> GetShipmentsAsync(Guid orderId)
    {
        await _orderRepository.GetAsync(orderId);
        var list = await _shipmentRepository.GetListAsync(s => s.OrderId == orderId);
        return list.OrderBy(s => s.CreationTime).Select(s => new ShipmentDto
        {
            Id = s.Id,
            OrderId = s.OrderId,
            Carrier = s.Carrier,
            TrackingNumber = s.TrackingNumber,
            ShippedAt = s.ShippedAt,
            Notes = s.Notes,
            CreationTime = s.CreationTime,
        }).ToList();
    }

    public async Task<ShipmentDto> CreateShipmentAsync(Guid orderId, CreateShipmentDto input)
    {
        var order = await _orderRepository.GetAsync(orderId);
        var shipment = new Shipment(
            GuidGenerator.Create(),
            orderId,
            input.Carrier,
            input.TrackingNumber,
            DateTime.UtcNow,
            input.Notes);
        await _shipmentRepository.InsertAsync(shipment);
        if (order.Status != OrderStatus.Shipped && order.Status != OrderStatus.Delivered)
        {
            order.SetStatus(OrderStatus.Shipped);
            await _orderRepository.UpdateAsync(order);
            var historyEntry = new OrderStatusHistory(GuidGenerator.Create(), order.Id, OrderStatus.Shipped);
            await _orderStatusHistoryRepository.InsertAsync(historyEntry);
        }
        var trackingInfo = string.IsNullOrWhiteSpace(input.TrackingNumber)
            ? null
            : string.IsNullOrWhiteSpace(input.Carrier)
                ? input.TrackingNumber
                : $"{input.Carrier}: {input.TrackingNumber}";
        await _transactionalEmail.SendShippingNotificationAsync(order.Id, order.ContactEmail, order.ContactName ?? "", trackingInfo);
        return new ShipmentDto
        {
            Id = shipment.Id,
            OrderId = shipment.OrderId,
            Carrier = shipment.Carrier,
            TrackingNumber = shipment.TrackingNumber,
            ShippedAt = shipment.ShippedAt,
            Notes = shipment.Notes,
            CreationTime = shipment.CreationTime,
        };
    }

    public async Task<RefundOrderResultDto> RefundOrderAsync(Guid orderId, decimal? amount = null, string? reason = null)
    {
        var order = await _orderRepository.GetAsync(orderId);
        var result = await _paymentAppService.RefundAsync(orderId, amount, reason);
        if (result.Success && order.Status >= OrderStatus.Confirmed)
        {
            order.SetStatus(OrderStatus.Cancelled);
            await _orderRepository.UpdateAsync(order);
            var historyEntry = new OrderStatusHistory(GuidGenerator.Create(), order.Id, OrderStatus.Cancelled);
            await _orderStatusHistoryRepository.InsertAsync(historyEntry);
            var lines = await _orderLineRepository.GetListAsync(l => l.OrderId == orderId);
            var toRestore = lines.Select(l => (l.ProductVariantId, l.Quantity)).ToList();
            await _inventoryDeduction.RestoreForOrderLinesAsync(toRestore);
        }
        return new RefundOrderResultDto
        {
            Success = result.Success,
            ErrorCode = result.ErrorCode,
            ErrorMessage = result.ErrorMessage,
        };
    }

    private static OrderDto MapToDto(Order order, List<OrderLine> lines, List<OrderStatusHistory> statusHistory)
    {
        var dto = new OrderDto
        {
            Id = order.Id,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            PaymentGateway = order.PaymentGateway,
            ExternalPaymentId = order.ExternalPaymentId,
            ContactEmail = order.ContactEmail,
            ContactPhone = order.ContactPhone,
            ContactName = order.ContactName,
            ShippingStreet = order.ShippingStreet,
            ShippingStreet2 = order.ShippingStreet2,
            ShippingCity = order.ShippingCity,
            ShippingRegion = order.ShippingRegion,
            ShippingPostalCode = order.ShippingPostalCode,
            ShippingCountry = order.ShippingCountry,
            ShippingMethodName = order.ShippingMethodName,
            SubTotal = order.SubTotal,
            ShippingAmount = order.ShippingAmount,
            TaxAmount = order.TaxAmount,
            Total = order.Total,
            CreationTime = order.CreationTime,
        };
        dto.Lines = lines.Select(l => new OrderLineDto
        {
            Id = l.Id,
            ProductVariantId = l.ProductVariantId,
            ProductId = l.ProductId,
            ProductName = l.ProductName,
            Sku = l.Sku,
            UnitPrice = l.UnitPrice,
            Quantity = l.Quantity,
            LineTotal = l.LineTotal,
        }).ToList();
        dto.StatusHistory = statusHistory.Select(h => new OrderStatusHistoryDto
        {
            Id = h.Id,
            OrderId = h.OrderId,
            Status = h.Status.ToString(),
            CreationTime = h.CreationTime,
        }).ToList();
        return dto;
    }
}
