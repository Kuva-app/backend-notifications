using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Enums;
using Microsoft.Extensions.Logging;

namespace Kuva.Email.Business.Providers;

public sealed class FakeEmailSender(ILogger<FakeEmailSender> logger) : IEmailSender
{
    public EmailProviderType ProviderType => EmailProviderType.Fake;

    public Task<EmailSendResult> SendAsync(RenderedEmail email, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fake email sent to {RecipientCount} recipient(s) with subject {Subject}.", email.Recipients.Count, email.Subject);
        return Task.FromResult(EmailSendResult.Ok($"fake-{Guid.NewGuid():N}"));
    }
}
