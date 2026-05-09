namespace Kuva.Notifications.Entities.Entities;

public sealed class NotificationRecipient
{
    public Guid Id { get; set; }
    public Guid NotificationRequestId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Role { get; set; } = "To";
    public DateTime CreatedAtUtc { get; set; }

    public NotificationRequest? NotificationRequest { get; set; }
}
