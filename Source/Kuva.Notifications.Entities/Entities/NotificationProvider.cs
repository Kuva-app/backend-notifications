using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Entities.Entities;

public sealed class NotificationProvider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Email;
    public NotificationProviderType ProviderType { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 1;
    public string? ConfigurationKey { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<NotificationSendAttempt> Attempts { get; set; } = [];
}
