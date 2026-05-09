using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Entities.Entities;

public sealed class NotificationEvent
{
    public Guid Id { get; set; }
    public Guid NotificationRequestId { get; set; }
    public NotificationEventType EventType { get; set; }
    public string? Description { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public NotificationRequest? NotificationRequest { get; set; }
}
