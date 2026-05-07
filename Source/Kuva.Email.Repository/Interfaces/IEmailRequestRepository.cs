using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Repository.Interfaces;

public interface IEmailRequestRepository
{
    Task AddAsync(EmailRequest request, CancellationToken cancellationToken);
    Task<EmailRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<EmailRequest?> GetByIdempotencyKeyAsync(string templateCode, string externalReference, string primaryRecipient, DateTime notBeforeUtc, CancellationToken cancellationToken);
    Task<int> GetAttemptCountAsync(Guid requestId, CancellationToken cancellationToken);
    Task AddAttemptAsync(EmailSendAttempt attempt, CancellationToken cancellationToken);
    Task AddEventAsync(EmailEvent emailEvent, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid requestId, EmailRequestStatus status, string? errorMessage, DateTime nowUtc, CancellationToken cancellationToken);
}
