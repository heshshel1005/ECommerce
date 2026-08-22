using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Orders;

/// <summary>
/// Order API for current user: list my orders, get order detail.
/// API exposed via OrderController only (not auto-exposed by ABP).
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize]
public class OrderAppService : ECommerceAppService, IOrderAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<OrderLine, Guid> _orderLineRepository;
    private readonly IRepository<OrderStatusHistory, Guid> _orderStatusHistoryRepository;

    public OrderAppService(
        IRepository<Order, Guid> orderRepository,
        IRepository<OrderLine, Guid> orderLineRepository,
        IRepository<OrderStatusHistory, Guid> orderStatusHistoryRepository)
    {
        _orderRepository = orderRepository;
        _orderLineRepository = orderLineRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync()
    {
        var userId = CurrentUser.Id ?? throw new Volo.Abp.Authorization.AbpAuthorizationException("User must be logged in to view orders.");
        var email = CurrentUser.Email;

        // Include orders explicitly placed by this user, plus guest orders that used this email (case-insensitive).
        var orders = await _orderRepository.GetListAsync(o =>
            o.UserId == userId ||
            (o.UserId == null && email != null && o.ContactEmail != null &&
             o.ContactEmail.ToLower() == email.ToLower()));
        orders = orders.OrderByDescending(o => o.CreationTime).ToList();
        return orders.Select(o => MapToDto(o)).ToList();
    }

    public async Task<OrderDto?> GetAsync(Guid id)
    {
        var userId = CurrentUser.Id;
        var order = await _orderRepository.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return null;
        if (order.UserId != userId && !await AuthorizationService.IsGrantedAsync("ECommerce.Administration"))
            return null;
        var lines = await _orderLineRepository.GetListAsync(l => l.OrderId == id);
        var history = await _orderStatusHistoryRepository.GetListAsync(h => h.OrderId == id);
        return MapToDto(order, lines, history.OrderBy(h => h.CreationTime).ToList());
    }

    private static OrderDto MapToDto(Order order, List<OrderLine>? lines = null, List<OrderStatusHistory>? statusHistory = null)
    {
        var dto = new OrderDto
        {
            Id = order.Id,
            Status = order.Status.ToString(),
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
            PaymentStatus = order.PaymentStatus.ToString(),
            PaymentGateway = order.PaymentGateway,
            ExternalPaymentId = order.ExternalPaymentId,
            CreationTime = order.CreationTime,
        };
        var lineList = lines ?? (order.Lines?.ToList() ?? new List<OrderLine>());
        dto.Lines = lineList.Select(l => new OrderLineDto
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
        if (statusHistory != null)
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
