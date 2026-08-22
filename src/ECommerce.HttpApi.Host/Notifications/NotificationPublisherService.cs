using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Hubs;
using ECommerce.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace ECommerce;

public class NotificationPublisherService : INotificationPublisherService
{
    private readonly IRepository<UserNotification, Guid> _notificationRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<NotificationPublisherService> _logger;

    public NotificationPublisherService(
        IRepository<UserNotification, Guid> notificationRepository,
        IIdentityUserRepository userRepository,
        IHubContext<NotificationHub> hubContext,
        ICurrentTenant currentTenant,
        ILogger<NotificationPublisherService> logger)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _hubContext = hubContext;
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public async Task PublishNotificationAsync(CreateNotificationDto input)
    {
        if (input.UserId.HasValue)
            await PublishNotificationToUserAsync(input.UserId.Value, input);
        else
            await PublishNotificationToAllUsersAsync(input);
    }

    public async Task PublishNotificationToUserAsync(Guid userId, CreateNotificationDto input)
    {
        var notification = new UserNotification(
            Guid.NewGuid(),
            _currentTenant.Id,
            userId,
            input.Title,
            input.Message,
            input.NotificationType ?? "Info",
            input.LinkUrl,
            input.Data);

        await _notificationRepository.InsertAsync(notification);

        try
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                message = notification.Message,
                notificationType = notification.NotificationType,
                linkUrl = notification.LinkUrl,
                notificationDate = notification.NotificationDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SignalR notification to user {UserId}", userId);
        }
    }

    public async Task PublishNotificationToAllUsersAsync(CreateNotificationDto input)
    {
        // ABP data filter scopes this to the current tenant automatically.
        var users = await _userRepository.GetListAsync();

        var notifications = users.Select(user => new UserNotification(
            Guid.NewGuid(),
            _currentTenant.Id,
            user.Id,
            input.Title,
            input.Message,
            input.NotificationType ?? "Info",
            input.LinkUrl,
            input.Data)).ToList();

        await _notificationRepository.InsertManyAsync(notifications);

        var tenantGroupName = _currentTenant.Id.HasValue
            ? $"tenant-{_currentTenant.Id.Value}"
            : "tenant-host";

        try
        {
            await _hubContext.Clients.Group(tenantGroupName).SendAsync("ReceiveNotification", new
            {
                title = input.Title,
                message = input.Message,
                notificationType = input.NotificationType ?? "Info",
                linkUrl = input.LinkUrl,
                notificationDate = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast SignalR notification to tenant group {Group}", tenantGroupName);
        }
    }
}
