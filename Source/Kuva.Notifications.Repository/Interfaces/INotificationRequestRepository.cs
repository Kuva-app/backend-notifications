using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Repository.Interfaces;

public interface INotificationRequestRepository
{
    Task AddAsync(NotificationRequest request, CancellationToken cancellationToken);
    Task<NotificationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<NotificationRequest?> GetByIdempotencyKeyAsync(NotificationType type, Guid templateId, string externalReference, string primaryRecipientAddress, DateTime notBeforeUtc, CancellationToken cancellationToken);
    Task<int> GetAttemptCountAsync(Guid requestId, CancellationToken cancellationToken);
    Task AddAttemptAsync(NotificationSendAttempt attempt, CancellationToken cancellationToken);
    Task AddEventAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid requestId, NotificationRequestStatus status, string? errorMessage, DateTime nowUtc, CancellationToken cancellationToken);
}
