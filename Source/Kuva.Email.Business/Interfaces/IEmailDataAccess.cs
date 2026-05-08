using Kuva.Email.Business.Services;
using Kuva.Email.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kuva.Email.Business.Interfaces;

public interface IEmailDataAccess
{
    IEmailTemplateRepository TemplateRepository { get; }
    IEmailRequestRepository RequestRepository { get; }
    IUnitOfWork UnitOfWork { get; }
    ITemplateRenderer TemplateRenderer { get; }
    IEmailValidationService ValidationService { get; }
    IEmailProviderFactory ProviderFactory { get; }
    IClock Clock { get; }
    IEmailMetrics Metrics { get; }
    ILogger<EmailBusiness> Logger { get; }
}
