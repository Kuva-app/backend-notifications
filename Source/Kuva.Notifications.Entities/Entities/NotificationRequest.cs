using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Entities.Entities;

public sealed class NotificationRequest
{
    public Guid Id { get; set; }
    public Guid? TemplateId { get; set; }
    public NotificationType Type { get; set; } = NotificationType.Email;
    public string TemplateCode { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public string? Source { get; set; }
    public NotificationRequestStatus Status { get; set; } = NotificationRequestStatus.Created;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string VariablesJson { get; set; } = "{}";
    public string? MetadataJson { get; set; }
    public string? SubjectRendered { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }

    public NotificationTemplate? Template { get; set; }
    public ICollection<NotificationRecipient> Recipients { get; set; } = [];
    public ICollection<NotificationAttachment> Attachments { get; set; } = [];
    public ICollection<NotificationSendAttempt> Attempts { get; set; } = [];
    public ICollection<NotificationEvent> Events { get; set; } = [];
}
