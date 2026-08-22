using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Catalog;
using ECommerce.Email;
using ECommerce.Marketing;
using ECommerce.Orders;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Payment;

/// <summary>
/// Payment app service. All operations require authentication. No raw card data is handled.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize]
public class PaymentAppService : ECommerceAppService, IPaymentAppService
{
    private readonly IEnumerable<IPaymentGateway> _gateways;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<OrderLine, Guid> _orderLineRepository;
    private readonly IRepository<OrderStatusHistory, Guid> _orderStatusHistoryRepository;
    private readonly ITransactionalEmailService _transactionalEmail;
    private readonly ILoyaltyPointsService _loyaltyPointsService;
    private readonly IInventoryDeductionService _inventoryDeduction;

    public PaymentAppService(
        IEnumerable<IPaymentGateway> gateways,
        IRepository<Order, Guid> orderRepository,
        IRepository<OrderLine, Guid> orderLineRepository,
        IRepository<OrderStatusHistory, Guid> orderStatusHistoryRepository,
        ITransactionalEmailService transactionalEmail,
        ILoyaltyPointsService loyaltyPointsService,
        IInventoryDeductionService inventoryDeduction)
    {
        _gateways = gateways;
        _orderRepository = orderRepository;
        _orderLineRepository = orderLineRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _transactionalEmail = transactionalEmail;
        _loyaltyPointsService = loyaltyPointsService;
        _inventoryDeduction = inventoryDeduction;
    }

    public Task<List<PaymentGatewayDto>> GetGatewaysAsync()
    {
        var list = _gateways.Select(g => new PaymentGatewayDto
        {
            Name = g.Name,
            DisplayName = g.Name == CashOnDeliveryPaymentGateway.GatewayName ? "Cash on Delivery" : g.Name,
            PublishableKeyOrClientId = g.PublishableKeyOrClientId
        }).ToList();
        return Task.FromResult(list);
    }

    public async Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(Guid orderId, string gatewayName)
    {
        var order = await _orderRepository.GetAsync(orderId);
        await EnsureOrderAccessAsync(order);
        if (order.PaymentStatus == PaymentStatus.Paid)
            return new CreatePaymentIntentResult { Success = false, ErrorCode = "AlreadyPaid", ErrorMessage = "Order is already paid." };

        var gateway = GetGateway(gatewayName);
        var request = new CreatePaymentIntentRequest
        {
            OrderId = orderId,
            Amount = order.Total,
            Currency = "usd",
            CustomerEmail = order.ContactEmail,
            Description = $"Order {orderId}"
        };
        return await gateway.CreatePaymentIntentAsync(request);
    }

    public async Task<ConfirmPaymentResult> ConfirmPaymentAsync(Guid orderId, string gatewayPaymentId)
    {
        var order = await _orderRepository.GetAsync(orderId);
        await EnsureOrderAccessAsync(order);

        var gatewayName = order.PaymentGateway ?? InferGatewayFromPaymentId(gatewayPaymentId);
        var gateway = GetGateway(gatewayName);

        var result = await gateway.ConfirmPaymentAsync(new ConfirmPaymentRequest { OrderId = orderId, GatewayPaymentId = gatewayPaymentId });
        if (!result.Success)
            return result;

        var isCashOnDelivery = gateway.Name == CashOnDeliveryPaymentGateway.GatewayName;
        order.SetPayment(gateway.Name, gatewayPaymentId, isCashOnDelivery ? PaymentStatus.CashOnDelivery : PaymentStatus.Paid);
        await _orderRepository.UpdateAsync(order);

        order.SetStatus(OrderStatus.Confirmed);
        await _orderRepository.UpdateAsync(order);
        var history = new OrderStatusHistory(GuidGenerator.Create(), order.Id, OrderStatus.Confirmed);
        await _orderStatusHistoryRepository.InsertAsync(history);

        var lines = await _orderLineRepository.GetListAsync(l => l.OrderId == order.Id);
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

        return result;
    }

    public async Task<RefundPaymentResult> RefundAsync(Guid orderId, decimal? amount = null, string? reason = null)
    {
        await AuthorizationService.CheckAsync("ECommerce.Administration");
        var order = await _orderRepository.GetAsync(orderId);
        if (string.IsNullOrEmpty(order.PaymentGateway) || string.IsNullOrEmpty(order.ExternalPaymentId))
            return new RefundPaymentResult { Success = false, ErrorCode = "NoPayment", ErrorMessage = "Order has no payment to refund." };
        if (order.PaymentStatus != PaymentStatus.Paid)
            return new RefundPaymentResult { Success = false, ErrorCode = "NotPaid", ErrorMessage = "Order is not in paid status." };

        var gateway = GetGateway(order.PaymentGateway);
        var request = new RefundPaymentRequest
        {
            OrderId = orderId,
            GatewayPaymentId = order.ExternalPaymentId,
            Amount = amount,
            Reason = reason
        };
        var result = await gateway.RefundPaymentAsync(request);
        if (result.Success)
        {
            order.SetPayment(order.PaymentGateway, order.ExternalPaymentId, PaymentStatus.Refunded);
            await _orderRepository.UpdateAsync(order);
        }
        return result;
    }

    private async Task EnsureOrderAccessAsync(Order order)
    {
        var userId = CurrentUser.Id;
        if (order.UserId == userId) return;
        if (order.UserId == null && CurrentUser.Email != null && order.ContactEmail?.Equals(CurrentUser.Email, StringComparison.OrdinalIgnoreCase) == true) return;
        if (await AuthorizationService.IsGrantedAsync("ECommerce.Administration")) return;
        throw new Volo.Abp.Authorization.AbpAuthorizationException("You do not have access to this order.");
    }

    private IPaymentGateway GetGateway(string name)
    {
        var g = _gateways.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (g == null)
            throw new Volo.Abp.BusinessException("ECommerce:UnknownPaymentGateway").WithData("Gateway", name);
        return g;
    }

    private static string InferGatewayFromPaymentId(string gatewayPaymentId)
    {
        if (string.Equals(gatewayPaymentId, "COD", StringComparison.OrdinalIgnoreCase))
            return CashOnDeliveryPaymentGateway.GatewayName;
        if (!string.IsNullOrEmpty(gatewayPaymentId) && gatewayPaymentId.Length <= 20 && gatewayPaymentId.All(char.IsDigit))
            return PaymobPaymentGateway.GatewayName;
        return PayPalPaymentGateway.GatewayName;
    }
}
