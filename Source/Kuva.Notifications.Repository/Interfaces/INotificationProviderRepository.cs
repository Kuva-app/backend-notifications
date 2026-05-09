using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Repository.Interfaces;

public interface INotificationProviderRepository
{
    Task<NotificationProvider?> GetActiveByPriorityAsync(NotificationType type, CancellationToken cancellationToken);
    Task<NotificationProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
