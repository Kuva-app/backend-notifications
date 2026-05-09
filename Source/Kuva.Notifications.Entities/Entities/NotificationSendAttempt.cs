namespace Kuva.Notifications.Entities.Entities;

public sealed class NotificationSendAttempt
{
    public Guid Id { get; set; }
    public Guid NotificationRequestId { get; set; }
    public Guid? NotificationProviderId { get; set; }
    public int AttemptNumber { get; set; }
    public bool Success { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? FinishedAtUtc { get; set; }

    public NotificationRequest? NotificationRequest { get; set; }
    public NotificationProvider? NotificationProvider { get; set; }
}
