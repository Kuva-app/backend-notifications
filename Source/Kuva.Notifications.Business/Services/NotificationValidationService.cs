using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Constants;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Entities.ValueObjects;

namespace Kuva.Notifications.Business.Services;

public sealed class NotificationValidationService : INotificationValidationService
{
    public NotificationValidationResult ValidateSendRequest(SendNotificationRequestDto request)
    {
        var result = NotificationValidationResult.Success();

        ValidateNotificationType(request.Type, result);
        ValidateTemplateId(request.TemplateId, result);
        ValidatePriority(request.Priority, result);
        ValidateRecipients(request.Type, request.Recipients, result);
        ValidateVariables(request.Variables, result);

        return result;
    }

    public NotificationValidationResult ValidateTemplate(NotificationTemplateDto template)
    {
        var result = NotificationValidationResult.Success();

        if (string.IsNullOrWhiteSpace(template.Code))
        {
            result.Errors.Add("Template code is required.");
        }

        ValidateNotificationType(template.Type, result);

        if (string.IsNullOrWhiteSpace(template.Name))
        {
            result.Errors.Add("Template name is required.");
        }

        if (template.Type == NotificationType.Email && string.IsNullOrWhiteSpace(template.SubjectTemplate))
        {
            result.Errors.Add("Subject template is required for email notifications.");
        }

        if (string.IsNullOrWhiteSpace(template.HtmlBodyTemplate))
        {
            result.Errors.Add("Body template is required.");
        }

        if (template.Version <= 0)
        {
            result.Errors.Add("Template version must be greater than zero.");
        }

        return result;
    }

    private static void ValidateNotificationType(NotificationType type, NotificationValidationResult result)
    {
        if (!Enum.IsDefined(type))
        {
            result.Errors.Add("Notification type is invalid.");
        }
    }

    private static void ValidateTemplateId(Guid templateId, NotificationValidationResult result)
    {
        if (templateId == Guid.Empty)
        {
            result.Errors.Add("TemplateId is required.");
        }
    }

    private static void ValidatePriority(NotificationPriority priority, NotificationValidationResult result)
    {
        if (!Enum.IsDefined(priority))
        {
            result.Errors.Add("Priority is invalid.");
        }
    }

    private void ValidateRecipients(NotificationType type, List<NotificationRecipientDto> recipients, NotificationValidationResult result)
    {
        if (recipients is null || recipients.Count == 0)
        {
            result.Errors.Add("At least one recipient is required.");
            return;
        }

        if (recipients.Count > NotificationConstants.MaxRecipients)
        {
            result.Errors.Add($"Recipients cannot exceed {NotificationConstants.MaxRecipients}.");
            return;
        }

        if (type == NotificationType.Email && !recipients.Any(x => x.Role.Equals("To", StringComparison.OrdinalIgnoreCase)))
        {
            result.Errors.Add("At least one To recipient is required.");
        }

        foreach (var recipient in recipients)
        {
            ValidateRecipient(type, recipient, result);
        }
    }

    private void ValidateRecipient(NotificationType type, NotificationRecipientDto recipient, NotificationValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(recipient.Address))
        {
            result.Errors.Add("Recipient address is required.");
            return;
        }

        if (recipient.Address.Length > NotificationConstants.MaxAddressLength)
        {
            result.Errors.Add($"Recipient address is too long: {recipient.Address}");
        }

        if (type == NotificationType.Email && !EmailAddress.IsValid(recipient.Address))
        {
            result.Errors.Add($"Recipient email is invalid: {recipient.Address}");
        }

        if (type == NotificationType.Email && !NotificationConstants.AllowedRecipientTypes.Contains(recipient.Role, StringComparer.OrdinalIgnoreCase))
        {
            result.Errors.Add($"Recipient role is invalid: {recipient.Role}");
        }
    }

    private void ValidateVariables(Dictionary<string, string> variables, NotificationValidationResult result)
    {
        if (variables is null)
        {
            result.Errors.Add("Variables cannot be null.");
            return;
        }

        foreach (var variable in variables)
        {
            if (variable.Value is not null && variable.Value.Length > NotificationConstants.MaxVariableValueLength)
            {
                result.Errors.Add($"Variable value is too long: {variable.Key}");
            }
        }
    }
}
