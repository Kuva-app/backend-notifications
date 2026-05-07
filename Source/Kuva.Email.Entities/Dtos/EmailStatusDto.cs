using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Entities.Dtos;

public sealed class EmailStatusDto
{
    public Guid Id { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public EmailRequestStatus Status { get; set; }
    public string? ExternalReference { get; set; }
    public string? Source { get; set; }
    public string? SubjectRendered { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
}
