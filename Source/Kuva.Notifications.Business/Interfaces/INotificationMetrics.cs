using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Business.Interfaces;

public interface INotificationMetrics
{
    void RequestReceived();
    void SendCompleted(NotificationRequestStatus status, string providerName, double durationSeconds);
    void ProviderFailure(string providerName);
}
