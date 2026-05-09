using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Entities.Entities;

public sealed class NotificationTemplate
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; } = NotificationType.Email;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SubjectTemplate { get; set; } = string.Empty;
    public string HtmlBodyTemplate { get; set; } = string.Empty;
    public string? TextBodyTemplate { get; set; }
    public string RequiredVariablesJson { get; set; } = "[]";
    public string Language { get; set; } = "pt-BR";
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<NotificationRequest> Requests { get; set; } = [];
}
