using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Entities.Dtos;

public sealed class SendNotificationRequestDto
{
    public NotificationType Type { get; set; } = NotificationType.Email;
    public Guid TemplateId { get; set; }
    public string? ExternalReference { get; set; }
    public string? Source { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public List<NotificationRecipientDto> Recipients { get; set; } = [];
    public Dictionary<string, string> Variables { get; set; } = [];
    public Dictionary<string, string>? Metadata { get; set; }
}
