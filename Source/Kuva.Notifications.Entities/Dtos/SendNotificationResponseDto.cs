using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Entities.Dtos;

public sealed class SendNotificationResponseDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public Guid? TemplateId { get; set; }
    public NotificationRequestStatus Status { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IdempotentReplay { get; set; }
}
