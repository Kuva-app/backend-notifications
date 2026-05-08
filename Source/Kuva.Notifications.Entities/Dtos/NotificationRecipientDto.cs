namespace Kuva.Notifications.Entities.Dtos;

public sealed class NotificationRecipientDto
{
    public string Address { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Role { get; set; } = "To";
}
