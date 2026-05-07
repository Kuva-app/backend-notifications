using Kuva.Email.Entities.Entities;
using Kuva.Email.Repository.Context;
using Kuva.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Email.Repository.Repositories;

public sealed class EmailTemplateRepository(EmailDbContext dbContext) : IEmailTemplateRepository
{
    public Task AddAsync(EmailTemplate template, CancellationToken cancellationToken)
        => dbContext.EmailTemplates.AddAsync(template, cancellationToken).AsTask();

    public Task<EmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.EmailTemplates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<EmailTemplate?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken)
        => dbContext.EmailTemplates
            .Where(x => x.Code == code && x.IsActive)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<EmailTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        => dbContext.EmailTemplates
            .Where(x => x.Code == code)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> CodeVersionExistsAsync(string code, int version, Guid? exceptId, CancellationToken cancellationToken)
        => dbContext.EmailTemplates.AnyAsync(x => x.Code == code && x.Version == version && x.Id != exceptId, cancellationToken);

    public void Update(EmailTemplate template) => dbContext.EmailTemplates.Update(template);
}
