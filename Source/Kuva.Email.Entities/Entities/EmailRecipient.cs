namespace Kuva.Email.Entities.Entities;

public sealed class EmailRecipient
{
    public Guid Id { get; set; }
    public Guid EmailRequestId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Type { get; set; } = "To";
    public DateTime CreatedAtUtc { get; set; }

    public EmailRequest? EmailRequest { get; set; }
}
