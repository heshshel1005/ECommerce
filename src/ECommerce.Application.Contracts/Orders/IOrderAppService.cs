using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Orders;

/// <summary>
/// Order API for current user: list my orders, get order detail.
/// API is exposed via OrderController.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
public interface IOrderAppService : IApplicationService
{
    Task<List<OrderDto>> GetMyOrdersAsync();
    Task<OrderDto?> GetAsync(Guid id);
}
