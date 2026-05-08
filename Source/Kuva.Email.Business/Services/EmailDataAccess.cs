using Kuva.Email.Business.Interfaces;
using Kuva.Email.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kuva.Email.Business.Services;

public sealed class EmailDataAccess(
    IEmailTemplateRepository templateRepository,
    IEmailRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ITemplateRenderer templateRenderer,
    IEmailValidationService validationService,
    IEmailProviderFactory providerFactory,
    IClock clock,
    IEmailMetrics metrics,
    ILogger<EmailBusiness> logger) : IEmailDataAccess
{
    public IEmailTemplateRepository TemplateRepository { get; } = templateRepository;
    public IEmailRequestRepository RequestRepository { get; } = requestRepository;
    public IUnitOfWork UnitOfWork { get; } = unitOfWork;
    public ITemplateRenderer TemplateRenderer { get; } = templateRenderer;
    public IEmailValidationService ValidationService { get; } = validationService;
    public IEmailProviderFactory ProviderFactory { get; } = providerFactory;
    public IClock Clock { get; } = clock;
    public IEmailMetrics Metrics { get; } = metrics;
    public ILogger<EmailBusiness> Logger { get; } = logger;
}
