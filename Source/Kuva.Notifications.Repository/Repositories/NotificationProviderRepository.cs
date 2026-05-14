using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Context;
using Kuva.Notifications.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Notifications.Repository.Repositories;

public sealed class NotificationProviderRepository(NotificationsDbContext dbContext) : INotificationProviderRepository
{
    public Task<NotificationProvider?> GetActiveByPriorityAsync(NotificationType type, CancellationToken cancellationToken)
        => dbContext.NotificationProviders
            .AsNoTracking()
            .Where(x => x.Type == type && x.IsActive)
            .OrderBy(x => x.Priority)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<NotificationProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.NotificationProviders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
