using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Enums;
using Microsoft.Extensions.Logging;

namespace Kuva.Notifications.Business.Providers;

public sealed class FakeEmailSender(ILogger<FakeEmailSender> logger) : INotificationSender
{
    public NotificationType Type => NotificationType.Email;
    public NotificationProviderType ProviderType => NotificationProviderType.Fake;

    public Task<NotificationSendResult> SendAsync(RenderedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fake email notification sent to {RecipientCount} recipient(s) with subject {Subject}.", notification.Recipients.Count, notification.Subject);
        return Task.FromResult(NotificationSendResult.Ok($"fake-{Guid.NewGuid():N}"));
    }
}
