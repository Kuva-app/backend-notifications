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

        ValidateTemplateCode(request.TemplateCode, result);
        ValidatePriority(request.Priority, result);
        ValidateRecipients(request.Recipients, result);
        ValidateVariables(request.Variables, result);

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

    private void ValidateTemplateCode(string templateCode, EmailValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(templateCode))
        {
            result.Errors.Add("TemplateCode is required.");
        }
        else if (templateCode.Length > EmailConstants.MaxTemplateCodeLength)
        {
            result.Errors.Add("TemplateCode is too long.");
        }
    }

    private static void ValidatePriority(EmailPriority priority, EmailValidationResult result)
    {
        if (!Enum.IsDefined(priority))
        {
            result.Errors.Add("Priority is invalid.");
        }
    }

    private void ValidateRecipients(List<EmailRecipientDto> recipients, EmailValidationResult result)
    {
        if (recipients is null || recipients.Count == 0)
        {
            result.Errors.Add("At least one recipient is required.");
            return;
        }

        if (recipients.Count > EmailConstants.MaxRecipients)
        {
            result.Errors.Add($"Recipients cannot exceed {EmailConstants.MaxRecipients}.");
            return;
        }

        if (!recipients.Any(x => x.Type.Equals("To", StringComparison.OrdinalIgnoreCase)))
        {
            result.Errors.Add("At least one To recipient is required.");
        }

        foreach (var recipient in recipients)
        {
            ValidateRecipient(recipient, result);
        }
    }

    private void ValidateRecipient(EmailRecipientDto recipient, EmailValidationResult result)
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

    private void ValidateVariables(Dictionary<string, string> variables, EmailValidationResult result)
    {
        if (variables is null)
        {
            result.Errors.Add("Variables cannot be null.");
            return;
        }

        foreach (var variable in variables)
        {
            if (variable.Value is not null && variable.Value.Length > EmailConstants.MaxVariableValueLength)
            {
                result.Errors.Add($"Variable value is too long: {variable.Key}");
            }
        }
    }
}
