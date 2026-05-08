namespace Kuva.Notifications.Entities.Entities;

public sealed class NotificationAttachment
{
    public Guid Id { get; set; }
    public Guid NotificationRequestId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageReference { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public NotificationRequest? NotificationRequest { get; set; }
}
