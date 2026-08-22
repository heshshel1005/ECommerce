using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Orders;

[Route("api/app/order")]
[Area("app")]
[Authorize]
public class OrderController : ECommerceController
{
    private readonly IOrderAppService _appService;

    public OrderController(IOrderAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("my-orders")]
    public async Task<List<OrderDto>> GetMyOrdersAsync()
    {
        return await _appService.GetMyOrdersAsync();
    }

    [HttpGet("{id}")]
    public async Task<OrderDto?> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }
}
