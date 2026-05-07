using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Entities.Dtos;

public sealed class SendEmailRequestDto
{
    public string TemplateCode { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public string? Source { get; set; }
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    public List<EmailRecipientDto> Recipients { get; set; } = [];
    public Dictionary<string, string> Variables { get; set; } = [];
    public Dictionary<string, string>? Metadata { get; set; }
}
