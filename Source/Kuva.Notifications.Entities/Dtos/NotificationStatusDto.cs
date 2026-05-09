using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Entities.Dtos;

public sealed class NotificationStatusDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public Guid? TemplateId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public NotificationRequestStatus Status { get; set; }
    public string? ExternalReference { get; set; }
    public string? Source { get; set; }
    public string? SubjectRendered { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
}
