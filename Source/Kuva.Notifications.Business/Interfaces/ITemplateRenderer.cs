using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Dtos;

namespace Kuva.Notifications.Business.Interfaces;

public interface ITemplateRenderer
{
    RenderedNotification Render(NotificationTemplate template, SendNotificationRequestDto request);
}
