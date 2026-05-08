using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Interfaces;

namespace Kuva.Notifications.Business.Services;

public sealed class NotificationProviderFactory(
    INotificationProviderRepository providerRepository,
    IEnumerable<INotificationSender> senders) : INotificationProviderFactory
{
    public async Task<SelectedNotificationProvider> GetActiveProviderAsync(NotificationType type, CancellationToken cancellationToken)
    {
        var provider = await providerRepository.GetActiveByPriorityAsync(type, cancellationToken)
            ?? throw new InvalidOperationException($"No active notification provider configured for {type}.");

        var sender = senders.FirstOrDefault(x => x.Type == type && x.ProviderType == provider.ProviderType)
            ?? throw new InvalidOperationException($"Notification provider sender is not registered: {type}/{provider.ProviderType}.");

        return new SelectedNotificationProvider
        {
            Provider = provider,
            Sender = sender
        };
    }
}
