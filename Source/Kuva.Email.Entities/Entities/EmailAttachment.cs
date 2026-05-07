namespace Kuva.Email.Entities.Entities;

public sealed class EmailAttachment
{
    public Guid Id { get; set; }
    public Guid EmailRequestId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageReference { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public EmailRequest? EmailRequest { get; set; }
}
