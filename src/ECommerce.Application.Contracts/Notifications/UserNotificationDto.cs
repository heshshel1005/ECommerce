using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Notifications;

public class UserNotificationDto : EntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? NotificationType { get; set; }
    public string? LinkUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadTime { get; set; }
    public DateTime NotificationDate { get; set; }
    public string? Data { get; set; }
    public DateTime CreationTime { get; set; }
}

public class CreateNotificationDto
{
    /// <summary>Target user. Null = broadcast to all users in the current tenant.</summary>
    public Guid? UserId { get; set; }
    public required string Title { get; set; }
    public string? Message { get; set; }
    public string? NotificationType { get; set; } = "Info";
    public string? LinkUrl { get; set; }
    public string? Data { get; set; }
}

public class GetNotificationsInput : PagedAndSortedResultRequestDto
{
    public bool? IsRead { get; set; }
    public string? NotificationType { get; set; }
}

public class NotificationCountDto
{
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }
}
