using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Orders;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Orders;

/// <summary>
/// Admin order API: list (with filters), detail, update status, shipments, refund.
/// </summary>
[Route("api/app/order-admin")]
[Area("app")]
public class OrderAdminController : ECommerceController
{
    private readonly IOrderAdminAppService _appService;

    public OrderAdminController(IOrderAdminAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<PagedResultDto<OrderListDto>> GetListAsync([FromQuery] OrderListRequestDto input)
    {
        return await _appService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public async Task<OrderDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpPut("{id}/status")]
    public async Task<OrderDto> UpdateStatusAsync(Guid id, [FromBody] UpdateOrderStatusDto input)
    {
        return await _appService.UpdateStatusAsync(id, input);
    }

    [HttpGet("{orderId}/shipments")]
    public async Task<List<ShipmentDto>> GetShipmentsAsync(Guid orderId)
    {
        return await _appService.GetShipmentsAsync(orderId);
    }

    [HttpPost("{orderId}/shipments")]
    public async Task<ShipmentDto> CreateShipmentAsync(Guid orderId, [FromBody] CreateShipmentDto input)
    {
        return await _appService.CreateShipmentAsync(orderId, input);
    }

    [HttpPost("{orderId}/refund")]
    public async Task<RefundOrderResultDto> RefundOrderAsync(Guid orderId, [FromQuery] decimal? amount = null, [FromQuery] string? reason = null)
    {
        return await _appService.RefundOrderAsync(orderId, amount, reason);
    }
}
