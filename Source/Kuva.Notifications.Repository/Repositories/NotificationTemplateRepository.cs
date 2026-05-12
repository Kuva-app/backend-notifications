using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Context;
using Kuva.Notifications.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Notifications.Repository.Repositories;

public sealed class NotificationTemplateRepository(NotificationsDbContext dbContext) : INotificationTemplateRepository
{
    public Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken)
        => dbContext.NotificationTemplates.AddAsync(template, cancellationToken).AsTask();

    public Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<NotificationTemplate?> GetActiveByIdAsync(Guid id, NotificationType type, CancellationToken cancellationToken)
        => dbContext.NotificationTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.Type == type && x.IsActive, cancellationToken);

    public Task<NotificationTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        => dbContext.NotificationTemplates
            .Where(x => x.Code == code && x.IsActive)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> CodeVersionExistsAsync(NotificationType type, string code, int version, Guid? exceptId, CancellationToken cancellationToken)
        => dbContext.NotificationTemplates.AnyAsync(x => x.Type == type && x.Code == code && x.Version == version && x.Id != exceptId, cancellationToken);

    public void Update(NotificationTemplate template) => dbContext.NotificationTemplates.Update(template);
}
