using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Dtos;

namespace Kuva.Notifications.Business.Interfaces;

public interface INotificationValidationService
{
    NotificationValidationResult ValidateSendRequest(SendNotificationRequestDto request);
    NotificationValidationResult ValidateTemplate(NotificationTemplateDto template);
}
