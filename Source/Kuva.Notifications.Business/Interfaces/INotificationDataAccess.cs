using Kuva.Notifications.Business.Services;
using Kuva.Notifications.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kuva.Notifications.Business.Interfaces;

public interface INotificationDataAccess
{
    INotificationTemplateRepository TemplateRepository { get; }
    INotificationRequestRepository RequestRepository { get; }
    IUnitOfWork UnitOfWork { get; }
    ITemplateRenderer TemplateRenderer { get; }
    INotificationValidationService ValidationService { get; }
    INotificationProviderFactory ProviderFactory { get; }
    IClock Clock { get; }
    INotificationMetrics Metrics { get; }
    ILogger<NotificationBusiness> Logger { get; }
}
