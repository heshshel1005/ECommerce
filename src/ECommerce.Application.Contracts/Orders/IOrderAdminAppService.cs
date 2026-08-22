using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ECommerce.Orders;

/// <summary>
/// Admin API: order list, detail, status update, shipments, refund.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
public interface IOrderAdminAppService : IApplicationService
{
    Task<PagedResultDto<OrderListDto>> GetListAsync(OrderListRequestDto input);
    Task<OrderDto> GetAsync(Guid id);
    Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input);
    Task<List<ShipmentDto>> GetShipmentsAsync(Guid orderId);
    Task<ShipmentDto> CreateShipmentAsync(Guid orderId, CreateShipmentDto input);
    /// <summary>Refund payment via gateway and optionally set order to Cancelled and restore inventory.</summary>
    Task<RefundOrderResultDto> RefundOrderAsync(Guid orderId, decimal? amount = null, string? reason = null);
}
