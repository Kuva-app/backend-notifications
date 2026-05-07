using Kuva.Email.Entities.Entities;

namespace Kuva.Email.Business.Models;

public sealed class SelectedEmailProvider
{
    public EmailProvider Provider { get; init; } = null!;
    public Interfaces.IEmailSender Sender { get; init; } = null!;
}
