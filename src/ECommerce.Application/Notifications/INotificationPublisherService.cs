using System;
using System.Threading.Tasks;

namespace ECommerce.Notifications;

public interface INotificationPublisherService
{
    Task PublishNotificationAsync(CreateNotificationDto input);
    Task PublishNotificationToUserAsync(Guid userId, CreateNotificationDto input);
    Task PublishNotificationToAllUsersAsync(CreateNotificationDto input);
}
