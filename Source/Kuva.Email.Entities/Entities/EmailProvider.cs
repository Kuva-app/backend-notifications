using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Entities.Entities;

public sealed class EmailProvider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmailProviderType ProviderType { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 1;
    public string? ConfigurationKey { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<EmailSendAttempt> Attempts { get; set; } = [];
}
