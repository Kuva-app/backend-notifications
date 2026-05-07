using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Enums;
using Kuva.Email.Repository.Context;
using Kuva.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Email.Repository.Repositories;

public sealed class EmailRequestRepository(EmailDbContext dbContext) : IEmailRequestRepository
{
    public Task AddAsync(EmailRequest request, CancellationToken cancellationToken)
        => dbContext.EmailRequests.AddAsync(request, cancellationToken).AsTask();

    public Task<EmailRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.EmailRequests
            .Include(x => x.Recipients)
            .Include(x => x.Attempts)
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<EmailRequest?> GetByIdempotencyKeyAsync(string templateCode, string externalReference, string primaryRecipient, DateTime notBeforeUtc, CancellationToken cancellationToken)
        => dbContext.EmailRequests
            .Include(x => x.Recipients)
            .Where(x => x.TemplateCode == templateCode &&
                x.ExternalReference == externalReference &&
                x.CreatedAtUtc >= notBeforeUtc &&
                x.Recipients.Any(r => r.Type == "To" && r.Email == primaryRecipient))
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<int> GetAttemptCountAsync(Guid requestId, CancellationToken cancellationToken)
        => dbContext.EmailSendAttempts.CountAsync(x => x.EmailRequestId == requestId, cancellationToken);

    public Task AddAttemptAsync(EmailSendAttempt attempt, CancellationToken cancellationToken)
        => dbContext.EmailSendAttempts.AddAsync(attempt, cancellationToken).AsTask();

    public Task AddEventAsync(EmailEvent emailEvent, CancellationToken cancellationToken)
        => dbContext.EmailEvents.AddAsync(emailEvent, cancellationToken).AsTask();

    public async Task UpdateStatusAsync(Guid requestId, EmailRequestStatus status, string? errorMessage, DateTime nowUtc, CancellationToken cancellationToken)
    {
        var request = await dbContext.EmailRequests.FirstAsync(x => x.Id == requestId, cancellationToken);
        request.Status = status;
        request.ErrorMessage = errorMessage;
        request.UpdatedAtUtc = nowUtc;
        request.SentAtUtc = status == EmailRequestStatus.Sent ? nowUtc : request.SentAtUtc;
    }
}
