namespace Kuva.Notifications.Entities.Enums;

public enum NotificationEventType
{
    Created = 1,
    TemplateNotFound = 2,
    ValidationFailed = 3,
    Rendered = 4,
    SendStarted = 5,
    SendSucceeded = 6,
    SendFailed = 7,
    StatusChanged = 8
}
