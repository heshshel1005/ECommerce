using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Notifications;

[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize]
public class UserNotificationAppService : ECommerceAppService, IUserNotificationAppService
{
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly IRepository<UserNotification, Guid> _repository;

    public UserNotificationAppService(
        IUserNotificationRepository notificationRepository,
        IRepository<UserNotification, Guid> repository)
    {
        _notificationRepository = notificationRepository;
        _repository = repository;
    }

    public async Task<UserNotificationDto> GetAsync(Guid id)
    {
        var notification = await _repository.GetAsync(id);
        return MapToDto(notification);
    }

    public async Task<PagedResultDto<UserNotificationDto>> GetListAsync(GetNotificationsInput input)
    {
        var userId = CurrentUser.Id;
        if (userId == null)
            return new PagedResultDto<UserNotificationDto>();

        var notifications = await _notificationRepository.GetUserNotificationsAsync(
            userId.Value,
            input.IsRead,
            input.SkipCount,
            input.MaxResultCount);

        var totalCount = await _repository.CountAsync(n =>
            n.UserId == userId.Value &&
            n.IsActive &&
            (input.IsRead == null || n.IsRead == input.IsRead) &&
            (string.IsNullOrEmpty(input.NotificationType) || n.NotificationType == input.NotificationType));

        return new PagedResultDto<UserNotificationDto>(
            totalCount,
            notifications.Select(MapToDto).ToList());
    }

    public async Task<NotificationCountDto> GetUnreadCountAsync()
    {
        var userId = CurrentUser.Id;
        if (userId == null)
            return new NotificationCountDto();

        var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId.Value);
        var totalCount = await _repository.CountAsync(n => n.UserId == userId.Value && n.IsActive);

        return new NotificationCountDto
        {
            UnreadCount = unreadCount,
            TotalCount = (int)totalCount
        };
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new AbpAuthorizationException("User must be logged in.");
        var notification = await _repository.GetAsync(id);
        if (notification.UserId != userId)
            throw new AbpAuthorizationException("You can only mark your own notifications as read.");

        await _notificationRepository.MarkAsReadAsync(id);
    }

    public async Task MarkAllAsReadAsync()
    {
        var userId = CurrentUser.Id ?? throw new AbpAuthorizationException("User must be logged in.");
        await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new AbpAuthorizationException("User must be logged in.");
        var notification = await _repository.GetAsync(id);
        if (notification.UserId != userId)
            throw new AbpAuthorizationException("You can only delete your own notifications.");

        notification.IsActive = false;
        await _repository.UpdateAsync(notification);
    }

    public async Task DeleteAllAsync()
    {
        var userId = CurrentUser.Id ?? throw new AbpAuthorizationException("User must be logged in.");
        var notifications = await _repository.GetListAsync(n => n.UserId == userId && n.IsActive);
        foreach (var n in notifications)
            n.IsActive = false;
        await _repository.UpdateManyAsync(notifications);
    }

    private static UserNotificationDto MapToDto(UserNotification n) => new()
    {
        Id = n.Id,
        TenantId = n.TenantId,
        UserId = n.UserId,
        Title = n.Title,
        Message = n.Message,
        NotificationType = n.NotificationType,
        LinkUrl = n.LinkUrl,
        IsRead = n.IsRead,
        ReadTime = n.ReadTime,
        NotificationDate = n.NotificationDate,
        Data = n.Data,
        CreationTime = n.CreationTime,
    };
}
