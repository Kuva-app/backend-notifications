using Kuva.Notifications.Entities.Dtos;

namespace Kuva.Notifications.Business.Interfaces;

public interface INotificationBusiness
{
    Task<SendNotificationResponseDto> SendAsync(SendNotificationRequestDto request, CancellationToken cancellationToken);
    Task<NotificationStatusDto?> GetStatusAsync(Guid id, CancellationToken cancellationToken);
    Task<NotificationTemplateDto> CreateTemplateAsync(NotificationTemplateDto template, CancellationToken cancellationToken);
    Task<NotificationTemplateDto?> UpdateTemplateAsync(Guid id, NotificationTemplateDto template, CancellationToken cancellationToken);
    Task<bool> SetTemplateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken);
    Task<NotificationTemplateDto?> GetTemplateByCodeAsync(string code, CancellationToken cancellationToken);
}
