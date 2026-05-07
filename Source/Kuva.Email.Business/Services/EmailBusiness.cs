using System.Diagnostics;
using System.Text.Json;
using Kuva.Email.Business.Interfaces;
using Kuva.Email.Business.Models;
using Kuva.Email.Entities.Dtos;
using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Enums;
using Microsoft.Extensions.Logging;

namespace Kuva.Email.Business.Services;

public sealed class EmailBusiness(IEmailDataAccess emailDataAccess) : IEmailBusiness
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan IdempotencyWindow = TimeSpan.FromHours(24);

    public async Task<SendEmailResponseDto> SendAsync(SendEmailRequestDto request, CancellationToken cancellationToken)
    {
        emailDataAccess.metrics.RequestReceived();
        var stopwatch = Stopwatch.StartNew();
        var validation = emailDataAccess.validationService.ValidateSendRequest(request);

        if (!validation.IsValid)
        {
            emailDataAccess.metrics.SendCompleted(EmailRequestStatus.InvalidVariables, "none", stopwatch.Elapsed.TotalSeconds);
            return new SendEmailResponseDto
            {
                Status = EmailRequestStatus.InvalidVariables,
                TemplateCode = request.TemplateCode,
                Message = string.Join(" ", validation.Errors)
            };
        }

        var primaryRecipient = request.Recipients.First(x => x.Type.Equals("To", StringComparison.OrdinalIgnoreCase)).Email;
        if (!string.IsNullOrWhiteSpace(request.ExternalReference))
        {
            var existing = await emailDataAccess.requestRepository.GetByIdempotencyKeyAsync(
                request.TemplateCode,
                request.ExternalReference,
                primaryRecipient,
                emailDataAccess.clock.UtcNow.Subtract(IdempotencyWindow),
                cancellationToken);

            if (existing is not null)
            {
                emailDataAccess.logger.LogInformation("Returning idempotent email request {EmailRequestId}.", existing.Id);
                return ToSendResponse(existing, "Email request already processed.", idempotentReplay: true);
            }
        }

        var template = await emailDataAccess.templateRepository.GetActiveByCodeAsync(request.TemplateCode, cancellationToken);
        if (template is null)
        {
            var notFoundRequest = CreateEmailRequest(request, null, EmailRequestStatus.TemplateNotFound, "Template was not found or is inactive.");
            await emailDataAccess.requestRepository.AddAsync(notFoundRequest, cancellationToken);
            await emailDataAccess.unitOfWork.SaveChangesAsync(cancellationToken);
            emailDataAccess.metrics.SendCompleted(EmailRequestStatus.TemplateNotFound, "none", stopwatch.Elapsed.TotalSeconds);
            emailDataAccess.logger.LogWarning("Template {TemplateCode} was not found or inactive.", request.TemplateCode);
            return ToSendResponse(notFoundRequest, $"Template {request.TemplateCode} was not found or is inactive.");
        }

        RenderedEmail renderedEmail;
        try
        {
            renderedEmail = emailDataAccess.templateRenderer.Render(template, request);
        }
        catch (TemplateRenderingException ex)
        {
            var invalidRequest = CreateEmailRequest(request, template, EmailRequestStatus.InvalidVariables, "Required variables are missing.");
            invalidRequest.Events.Add(CreateEvent(invalidRequest.Id, EmailEventType.ValidationFailed, string.Join(",", ex.MissingVariables), null));
            await emailDataAccess.requestRepository.AddAsync(invalidRequest, cancellationToken);
            await emailDataAccess.unitOfWork.SaveChangesAsync(cancellationToken);
            emailDataAccess.metrics.SendCompleted(EmailRequestStatus.InvalidVariables, "none", stopwatch.Elapsed.TotalSeconds);
            return ToSendResponse(invalidRequest, "Required variables are missing.");
        }

        var emailRequest = CreateEmailRequest(request, template, EmailRequestStatus.PendingSend, null);
        emailRequest.SubjectRendered = renderedEmail.Subject;
        emailRequest.Events.Add(CreateEvent(emailRequest.Id, EmailEventType.Created, "Email request created.", null));
        emailRequest.Events.Add(CreateEvent(emailRequest.Id, EmailEventType.Rendered, "Email rendered.", null));

        await emailDataAccess.requestRepository.AddAsync(emailRequest, cancellationToken);
        await emailDataAccess.unitOfWork.SaveChangesAsync(cancellationToken);

        SelectedEmailProvider? selectedProvider = null;
        EmailSendResult sendResult;
        var startedAtUtc = emailDataAccess.clock.UtcNow;

        try
        {
            selectedProvider = await emailDataAccess.providerFactory.GetActiveProviderAsync(cancellationToken);
            emailRequest.Events.Add(CreateEvent(emailRequest.Id, EmailEventType.SendStarted, selectedProvider.Provider.Name, null));
            emailDataAccess.logger.LogInformation("Sending email request {EmailRequestId} using provider {Provider}.", emailRequest.Id, selectedProvider.Provider.Name);
            sendResult = await selectedProvider.Sender.SendAsync(renderedEmail, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            emailDataAccess.logger.LogError(ex, "Unexpected email provider failure for request {EmailRequestId}.", emailRequest.Id);
            sendResult = EmailSendResult.Fail("ProviderFailure", "Email provider failed unexpectedly.");
        }

        var finalStatus = sendResult.Success ? EmailRequestStatus.Sent : EmailRequestStatus.Failed;
        var safeError = sendResult.Success ? null : Sanitize(sendResult.ErrorMessage ?? "Email send failed.");
        emailRequest.Status = finalStatus;
        emailRequest.ErrorMessage = safeError;
        emailRequest.UpdatedAtUtc = emailDataAccess.clock.UtcNow;
        emailRequest.SentAtUtc = sendResult.Success ? emailDataAccess.clock.UtcNow : null;

        emailRequest.Attempts.Add(new EmailSendAttempt
        {
            Id = Guid.NewGuid(),
            EmailRequestId = emailRequest.Id,
            EmailProviderId = selectedProvider?.Provider.Id,
            AttemptNumber = 1,
            Success = sendResult.Success,
            ProviderMessageId = sendResult.ProviderMessageId,
            ErrorCode = sendResult.ErrorCode,
            ErrorMessage = safeError,
            StartedAtUtc = startedAtUtc,
            FinishedAtUtc = emailDataAccess.clock.UtcNow
        });

        emailRequest.Events.Add(CreateEvent(
            emailRequest.Id,
            sendResult.Success ? EmailEventType.SendSucceeded : EmailEventType.SendFailed,
            sendResult.Success ? "Email sent successfully." : safeError,
            null));

        await emailDataAccess.unitOfWork.SaveChangesAsync(cancellationToken);

        if (!sendResult.Success && selectedProvider is not null)
        {
            emailDataAccess.metrics.ProviderFailure(selectedProvider.Provider.Name);
        }

        emailDataAccess.metrics.SendCompleted(finalStatus, selectedProvider?.Provider.Name ?? "none", stopwatch.Elapsed.TotalSeconds);
        return ToSendResponse(emailRequest, sendResult.Success ? "Email sent successfully." : "Email send failed.");
    }

    public async Task<EmailStatusDto?> GetStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await emailDataAccess.requestRepository.GetByIdAsync(id, cancellationToken);
        return request is null ? null : ToStatusDto(request);
    }

    public async Task<EmailTemplateDto> CreateTemplateAsync(EmailTemplateDto template, CancellationToken cancellationToken)
    {
        var validation = emailDataAccess.validationService.ValidateTemplate(template);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        if (await emailDataAccess.templateRepository.CodeVersionExistsAsync(template.Code, template.Version, null, cancellationToken))
        {
            throw new InvalidOperationException("Template code and version already exist.");
        }

        var entity = ToTemplateEntity(template);
        await emailDataAccess.templateRepository.AddAsync(entity, cancellationToken);
        await emailDataAccess.unitOfWork.SaveChangesAsync(cancellationToken);
        return ToTemplateDto(entity);
    }

    public async Task<EmailTemplateDto?> UpdateTemplateAsync(Guid id, EmailTemplateDto template, CancellationToken cancellationToken)
    {   
        var entity = await emailDataAccess.templateRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var validation = emailDataAccess.validationService.ValidateTemplate(template);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        if (await emailDataAccess.templateRepository.CodeVersionExistsAsync(template.Code, template.Version, id, cancellationToken))
        {
            throw new InvalidOperationException("Template code and version already exist.");
        }

        entity.Code = template.Code;
        entity.Name = template.Name;
        entity.Description = template.Description;
        entity.SubjectTemplate = template.SubjectTemplate;
        entity.HtmlBodyTemplate = template.HtmlBodyTemplate;
        entity.TextBodyTemplate = template.TextBodyTemplate;
        entity.RequiredVariablesJson = JsonSerializer.Serialize(template.RequiredVariables, JsonOptions);
        entity.Language = template.Language;
        entity.Version = template.Version;
        entity.IsActive = template.IsActive;
        entity.UpdatedAtUtc = emailDataAccess.clock.UtcNow;

        emailDataAccess.templateRepository.Update(entity);
        await emailDataAccess.unitOfWork.SaveChangesAsync(cancellationToken);
        return ToTemplateDto(entity);
    }

    public async Task<bool> SetTemplateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var template = await emailDataAccess.templateRepository.GetByIdAsync(id, cancellationToken);
        if (template is null)
        {
            return false;
        }

        template.IsActive = isActive;
        template.UpdatedAtUtc = emailDataAccess.clock.UtcNow;
        emailDataAccess.templateRepository.Update(template);
        await emailDataAccess.unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<EmailTemplateDto?> GetTemplateByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var template = await emailDataAccess.templateRepository.GetByCodeAsync(code, cancellationToken);
        return template is null ? null : ToTemplateDto(template);
    }

    private EmailRequest CreateEmailRequest(SendEmailRequestDto request, EmailTemplate? template, EmailRequestStatus status, string? errorMessage)
    {
        var now = emailDataAccess.clock.UtcNow;
        var entity = new EmailRequest
        {
            Id = Guid.NewGuid(),
            TemplateId = template?.Id,
            TemplateCode = request.TemplateCode,
            ExternalReference = request.ExternalReference,
            Source = request.Source,
            Status = status,
            Priority = request.Priority,
            VariablesJson = JsonSerializer.Serialize(request.Variables, JsonOptions),
            MetadataJson = request.Metadata is null ? null : JsonSerializer.Serialize(request.Metadata, JsonOptions),
            ErrorMessage = errorMessage,
            CreatedAtUtc = now,
            UpdatedAtUtc = status is EmailRequestStatus.TemplateNotFound or EmailRequestStatus.InvalidVariables ? now : null
        };

        foreach (var recipient in request.Recipients)
        {
            entity.Recipients.Add(new EmailRecipient
            {
                Id = Guid.NewGuid(),
                EmailRequestId = entity.Id,
                Email = recipient.Email,
                Name = recipient.Name,
                Type = recipient.Type,
                CreatedAtUtc = now
            });
        }

        return entity;
    }

    private EmailEvent CreateEvent(Guid requestId, EmailEventType eventType, string? description, string? metadataJson)
        => new()
        {
            Id = Guid.NewGuid(),
            EmailRequestId = requestId,
            EventType = eventType,
            Description = Sanitize(description),
            MetadataJson = metadataJson,
            CreatedAtUtc = emailDataAccess.clock.UtcNow
        };

    private static string? Sanitize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Length <= 1000 ? value : value[..1000];
    }

    private static SendEmailResponseDto ToSendResponse(EmailRequest request, string message, bool idempotentReplay = false)
        => new()
        {
            Id = request.Id,
            Status = request.Status,
            TemplateCode = request.TemplateCode,
            Message = message,
            IdempotentReplay = idempotentReplay
        };

    private static EmailStatusDto ToStatusDto(EmailRequest request)
        => new()
        {
            Id = request.Id,
            TemplateCode = request.TemplateCode,
            Status = request.Status,
            ExternalReference = request.ExternalReference,
            Source = request.Source,
            SubjectRendered = request.SubjectRendered,
            ErrorMessage = request.ErrorMessage,
            CreatedAtUtc = request.CreatedAtUtc,
            SentAtUtc = request.SentAtUtc
        };

    private EmailTemplate ToTemplateEntity(EmailTemplateDto template)
        => new()
        {
            Id = template.Id ?? Guid.NewGuid(),
            Code = template.Code,
            Name = template.Name,
            Description = template.Description,
            SubjectTemplate = template.SubjectTemplate,
            HtmlBodyTemplate = template.HtmlBodyTemplate,
            TextBodyTemplate = template.TextBodyTemplate,
            RequiredVariablesJson = JsonSerializer.Serialize(template.RequiredVariables, JsonOptions),
            Language = template.Language,
            Version = template.Version,
            IsActive = template.IsActive,
            CreatedAtUtc = emailDataAccess.clock.UtcNow
        };

    private static EmailTemplateDto ToTemplateDto(EmailTemplate template)
        => new()
        {
            Id = template.Id,
            Code = template.Code,
            Name = template.Name,
            Description = template.Description,
            SubjectTemplate = template.SubjectTemplate,
            HtmlBodyTemplate = template.HtmlBodyTemplate,
            TextBodyTemplate = template.TextBodyTemplate,
            RequiredVariables = JsonSerializer.Deserialize<List<string>>(template.RequiredVariablesJson, JsonOptions) ?? [],
            Language = template.Language,
            Version = template.Version,
            IsActive = template.IsActive
        };
}
