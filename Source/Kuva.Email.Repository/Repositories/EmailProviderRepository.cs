using Kuva.Email.Entities.Entities;
using Kuva.Email.Repository.Context;
using Kuva.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Email.Repository.Repositories;

public sealed class EmailProviderRepository(EmailDbContext dbContext) : IEmailProviderRepository
{
    public Task<EmailProvider?> GetActiveByPriorityAsync(CancellationToken cancellationToken)
        => dbContext.EmailProviders
            .Where(x => x.IsActive)
            .OrderBy(x => x.Priority)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<EmailProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.EmailProviders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
