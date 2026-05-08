using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Context;
using Kuva.Notifications.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Notifications.Repository.Repositories;

public sealed class NotificationRequestRepository(NotificationsDbContext dbContext) : INotificationRequestRepository
{
    public Task AddAsync(NotificationRequest request, CancellationToken cancellationToken)
        => dbContext.NotificationRequests.AddAsync(request, cancellationToken).AsTask();

    public Task<NotificationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.NotificationRequests
            .Include(x => x.Recipients)
            .Include(x => x.Attempts)
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<NotificationRequest?> GetByIdempotencyKeyAsync(NotificationType type, Guid templateId, string externalReference, string primaryRecipientAddress, DateTime notBeforeUtc, CancellationToken cancellationToken)
        => dbContext.NotificationRequests
            .Include(x => x.Recipients)
            .Where(x => x.Type == type &&
                x.TemplateId == templateId &&
                x.ExternalReference == externalReference &&
                x.CreatedAtUtc >= notBeforeUtc &&
                x.Recipients.Any(r => r.Role == "To" && r.Address == primaryRecipientAddress))
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<int> GetAttemptCountAsync(Guid requestId, CancellationToken cancellationToken)
        => dbContext.NotificationSendAttempts.CountAsync(x => x.NotificationRequestId == requestId, cancellationToken);

    public Task AddAttemptAsync(NotificationSendAttempt attempt, CancellationToken cancellationToken)
        => dbContext.NotificationSendAttempts.AddAsync(attempt, cancellationToken).AsTask();

    public Task AddEventAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken)
        => dbContext.NotificationEvents.AddAsync(notificationEvent, cancellationToken).AsTask();

    public async Task UpdateStatusAsync(Guid requestId, NotificationRequestStatus status, string? errorMessage, DateTime nowUtc, CancellationToken cancellationToken)
    {
        var request = await dbContext.NotificationRequests.FirstAsync(x => x.Id == requestId, cancellationToken);
        request.Status = status;
        request.ErrorMessage = errorMessage;
        request.UpdatedAtUtc = nowUtc;
        request.SentAtUtc = status == NotificationRequestStatus.Sent ? nowUtc : request.SentAtUtc;
    }
}
