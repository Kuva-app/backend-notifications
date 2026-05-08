using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Business.Interfaces;

public interface INotificationProviderFactory
{
    Task<SelectedNotificationProvider> GetActiveProviderAsync(NotificationType type, CancellationToken cancellationToken);
}
