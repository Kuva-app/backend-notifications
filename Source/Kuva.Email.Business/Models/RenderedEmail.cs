using Kuva.Email.Entities.Dtos;

namespace Kuva.Email.Business.Models;

public sealed class RenderedEmail
{
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string? TextBody { get; set; }
    public List<EmailRecipientDto> Recipients { get; set; } = [];
}
