using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Dtos;

namespace Kuva.Email.Business.Interfaces;

public interface IEmailValidationService
{
    EmailValidationResult ValidateSendRequest(SendEmailRequestDto request);
    EmailValidationResult ValidateTemplate(EmailTemplateDto template);
}
