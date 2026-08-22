using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ECommerce.Notifications;

public interface IUserNotificationAppService : IApplicationService
{
    Task<UserNotificationDto> GetAsync(Guid id);
    Task<PagedResultDto<UserNotificationDto>> GetListAsync(GetNotificationsInput input);
    Task<NotificationCountDto> GetUnreadCountAsync();
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync();
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}
