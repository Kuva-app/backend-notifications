using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Business.Interfaces;

public interface INotificationSender
{
    NotificationType Type { get; }
    NotificationProviderType ProviderType { get; }
    Task<NotificationSendResult> SendAsync(RenderedNotification notification, CancellationToken cancellationToken);
}
