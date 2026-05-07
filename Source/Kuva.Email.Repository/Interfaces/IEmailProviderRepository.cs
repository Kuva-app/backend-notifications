using Kuva.Email.Entities.Entities;

namespace Kuva.Email.Repository.Interfaces;

public interface IEmailProviderRepository
{
    Task<EmailProvider?> GetActiveByPriorityAsync(CancellationToken cancellationToken);
    Task<EmailProvider?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
