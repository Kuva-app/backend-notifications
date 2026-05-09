using Kuva.Notifications.Entities.Dtos;

namespace Kuva.Notifications.Business.Models;

public sealed class RenderedNotification
{
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string? TextBody { get; set; }
    public List<NotificationRecipientDto> Recipients { get; set; } = [];
}
