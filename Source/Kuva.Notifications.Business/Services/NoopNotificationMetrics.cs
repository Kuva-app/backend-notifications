using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Business.Services;

public sealed class NoopNotificationMetrics : INotificationMetrics
{
    public void RequestReceived()
    {
    }

    public void SendCompleted(NotificationRequestStatus status, string providerName, double durationSeconds)
    {
    }

    public void ProviderFailure(string providerName)
    {
    }
}
