using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Entities.Entities;

public sealed class EmailRequest
{
    public Guid Id { get; set; }
    public Guid? TemplateId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public string? Source { get; set; }
    public EmailRequestStatus Status { get; set; } = EmailRequestStatus.Created;
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    public string VariablesJson { get; set; } = "{}";
    public string? MetadataJson { get; set; }
    public string? SubjectRendered { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }

    public EmailTemplate? Template { get; set; }
    public ICollection<EmailRecipient> Recipients { get; set; } = [];
    public ICollection<EmailAttachment> Attachments { get; set; } = [];
    public ICollection<EmailSendAttempt> Attempts { get; set; } = [];
    public ICollection<EmailEvent> Events { get; set; } = [];
}
