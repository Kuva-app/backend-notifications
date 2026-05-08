using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Repository.Interfaces;

public interface INotificationTemplateRepository
{
    Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken);
    Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<NotificationTemplate?> GetActiveByIdAsync(Guid id, NotificationType type, CancellationToken cancellationToken);
    Task<NotificationTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> CodeVersionExistsAsync(NotificationType type, string code, int version, Guid? exceptId, CancellationToken cancellationToken);
    void Update(NotificationTemplate template);
}
