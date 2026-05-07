using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Entities.Dtos;

public sealed class SendEmailResponseDto
{
    public Guid Id { get; set; }
    public EmailRequestStatus Status { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IdempotentReplay { get; set; }
}
