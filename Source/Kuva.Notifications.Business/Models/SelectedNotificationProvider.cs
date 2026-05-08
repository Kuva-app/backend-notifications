using Kuva.Notifications.Entities.Entities;

namespace Kuva.Notifications.Business.Models;

public sealed class SelectedNotificationProvider
{
    public NotificationProvider Provider { get; init; } = null!;
    public Interfaces.INotificationSender Sender { get; init; } = null!;
}
