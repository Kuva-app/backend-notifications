using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Constants;
using Kuva.Email.Entities.Dtos;
using Kuva.Email.Entities.Enums;
using Kuva.Email.Entities.ValueObjects;

namespace Kuva.Email.Business.Services;

public sealed class EmailValidationService : IEmailValidationService
{
    public EmailValidationResult ValidateSendRequest(SendEmailRequestDto request)
    {
        var result = EmailValidationResult.Success();

        if (string.IsNullOrWhiteSpace(request.TemplateCode))
        {
            result.Errors.Add("TemplateCode is required.");
        }
        else if (request.TemplateCode.Length > EmailConstants.MaxTemplateCodeLength)
        {
            result.Errors.Add("TemplateCode is too long.");
        }

        if (!Enum.IsDefined(request.Priority))
        {
            result.Errors.Add("Priority is invalid.");
        }

        if (request.Recipients is null || request.Recipients.Count == 0)
        {
            result.Errors.Add("At least one recipient is required.");
        }
        else if (request.Recipients.Count > EmailConstants.MaxRecipients)
        {
            result.Errors.Add($"Recipients cannot exceed {EmailConstants.MaxRecipients}.");
        }
        else
        {
            if (!request.Recipients.Any(x => x.Type.Equals("To", StringComparison.OrdinalIgnoreCase)))
            {
                result.Errors.Add("At least one To recipient is required.");
            }

            foreach (var recipient in request.Recipients)
            {
                if (!EmailAddress.IsValid(recipient.Email))
                {
                    result.Errors.Add($"Recipient email is invalid: {recipient.Email}");
                }

                if (!EmailConstants.AllowedRecipientTypes.Contains(recipient.Type, StringComparer.OrdinalIgnoreCase))
                {
                    result.Errors.Add($"Recipient type is invalid: {recipient.Type}");
                }
            }
        }

        if (request.Variables is null)
        {
            result.Errors.Add("Variables cannot be null.");
        }
        else
        {
            foreach (var variable in request.Variables)
            {
                if (variable.Value is not null && variable.Value.Length > EmailConstants.MaxVariableValueLength)
                {
                    result.Errors.Add($"Variable value is too long: {variable.Key}");
                }
            }
        }

        return result;
    }

    public EmailValidationResult ValidateTemplate(EmailTemplateDto template)
    {
        var result = EmailValidationResult.Success();

        if (string.IsNullOrWhiteSpace(template.Code))
        {
            result.Errors.Add("Template code is required.");
        }

        if (string.IsNullOrWhiteSpace(template.Name))
        {
            result.Errors.Add("Template name is required.");
        }

        if (string.IsNullOrWhiteSpace(template.SubjectTemplate))
        {
            result.Errors.Add("Subject template is required.");
        }

        if (string.IsNullOrWhiteSpace(template.HtmlBodyTemplate))
        {
            result.Errors.Add("HTML body template is required.");
        }

        if (template.Version <= 0)
        {
            result.Errors.Add("Template version must be greater than zero.");
        }

        return result;
    }
}
