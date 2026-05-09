namespace Kuva.Notifications.Entities.Enums;

public enum NotificationRequestStatus
{
    Created = 1,
    TemplateNotFound = 2,
    InvalidVariables = 3,
    PendingSend = 4,
    Sent = 5,
    Failed = 6,
    RetryScheduled = 7,
    Cancelled = 8
}
