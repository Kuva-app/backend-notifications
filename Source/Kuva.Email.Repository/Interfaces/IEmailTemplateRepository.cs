using Kuva.Email.Entities.Entities;

namespace Kuva.Email.Repository.Interfaces;

public interface IEmailTemplateRepository
{
    Task AddAsync(EmailTemplate template, CancellationToken cancellationToken);
    Task<EmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<EmailTemplate?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken);
    Task<EmailTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> CodeVersionExistsAsync(string code, int version, Guid? exceptId, CancellationToken cancellationToken);
    void Update(EmailTemplate template);
}
