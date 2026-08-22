using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.Notifications;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ECommerce.EntityFrameworkCore;

public class UserNotificationRepository :
    EfCoreRepository<ECommerceDbContext, UserNotification, Guid>,
    IUserNotificationRepository
{
    public UserNotificationRepository(IDbContextProvider<ECommerceDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<List<UserNotification>> GetUserNotificationsAsync(
        Guid userId,
        bool? isRead = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(n => n.UserId == userId && n.IsActive);

        if (isRead.HasValue)
            query = query.Where(n => n.IsRead == isRead.Value);

        return await query
            .OrderByDescending(n => n.NotificationDate)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query.CountAsync(
            n => n.UserId == userId && !n.IsRead && n.IsActive,
            cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await GetAsync(notificationId, cancellationToken: cancellationToken);
        notification.IsRead = true;
        notification.ReadTime = DateTime.UtcNow;
        await UpdateAsync(notification, cancellationToken: cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        var notifications = await query
            .Where(n => n.UserId == userId && !n.IsRead && n.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadTime = DateTime.UtcNow;
        }

        await UpdateManyAsync(notifications, cancellationToken: cancellationToken);
    }
}
