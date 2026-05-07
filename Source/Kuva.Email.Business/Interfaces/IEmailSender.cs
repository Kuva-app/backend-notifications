using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Business.Interfaces;

public interface IEmailSender
{
    EmailProviderType ProviderType { get; }
    Task<EmailSendResult> SendAsync(RenderedEmail email, CancellationToken cancellationToken);
}
