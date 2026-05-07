using Kuva.Email.Repository.Context;
using Kuva.Email.Repository.Interfaces;

namespace Kuva.Email.Repository.Repositories;

public sealed class UnitOfWork(EmailDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
