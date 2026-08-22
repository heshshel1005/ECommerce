using System;
using System.Threading.Tasks;
using ECommerce.Notifications;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Notifications;

[Route("api/app/notifications")]
public class UserNotificationController : AbpControllerBase
{
    private readonly IUserNotificationAppService _notificationAppService;

    public UserNotificationController(IUserNotificationAppService notificationAppService)
    {
        _notificationAppService = notificationAppService;
    }

    [HttpGet("{id}")]
    public async Task<UserNotificationDto> GetAsync(Guid id)
        => await _notificationAppService.GetAsync(id);

    [HttpGet]
    public async Task<PagedResultDto<UserNotificationDto>> GetListAsync([FromQuery] GetNotificationsInput input)
        => await _notificationAppService.GetListAsync(input);

    [HttpGet("unread-count")]
    public async Task<NotificationCountDto> GetUnreadCountAsync()
        => await _notificationAppService.GetUnreadCountAsync();

    [HttpPut("{id}/mark-as-read")]
    public async Task MarkAsReadAsync(Guid id)
        => await _notificationAppService.MarkAsReadAsync(id);

    [HttpPut("mark-all-as-read")]
    public async Task MarkAllAsReadAsync()
        => await _notificationAppService.MarkAllAsReadAsync();

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
        => await _notificationAppService.DeleteAsync(id);

    [HttpDelete("all")]
    public async Task DeleteAllAsync()
        => await _notificationAppService.DeleteAllAsync();
}
