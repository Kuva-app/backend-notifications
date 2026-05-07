using Kuva.Email.Business.Services;
using Kuva.Email.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kuva.Email.Business.Interfaces;

public interface IEmailDataAccess
{
    IEmailTemplateRepository templateRepository { get; }
    IEmailRequestRepository requestRepository { get; }
    IUnitOfWork unitOfWork { get; }
    ITemplateRenderer templateRenderer { get; }
    IEmailValidationService validationService { get; }
    IEmailProviderFactory providerFactory { get; }
    IClock clock { get; }
    IEmailMetrics metrics { get; }
    ILogger<EmailBusiness> logger { get; }
}