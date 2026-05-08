using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kuva.Notifications.Business.Services;

public sealed class NotificationDataAccess(
    INotificationTemplateRepository templateRepository,
    INotificationRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ITemplateRenderer templateRenderer,
    INotificationValidationService validationService,
    INotificationProviderFactory providerFactory,
    IClock clock,
    INotificationMetrics metrics,
    ILogger<NotificationBusiness> logger) : INotificationDataAccess
{
    public INotificationTemplateRepository TemplateRepository { get; } = templateRepository;
    public INotificationRequestRepository RequestRepository { get; } = requestRepository;
    public IUnitOfWork UnitOfWork { get; } = unitOfWork;
    public ITemplateRenderer TemplateRenderer { get; } = templateRenderer;
    public INotificationValidationService ValidationService { get; } = validationService;
    public INotificationProviderFactory ProviderFactory { get; } = providerFactory;
    public IClock Clock { get; } = clock;
    public INotificationMetrics Metrics { get; } = metrics;
    public ILogger<NotificationBusiness> Logger { get; } = logger;
}
