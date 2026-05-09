using Kuva.Notifications.Repository.Context;
using Kuva.Notifications.Repository.Interfaces;

namespace Kuva.Notifications.Repository.Repositories;

public sealed class UnitOfWork(NotificationsDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
