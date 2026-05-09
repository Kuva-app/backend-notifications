using System.Diagnostics;
using System.Text.Json;
using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kuva.Notifications.Business.Services;

public sealed class NotificationBusiness(INotificationDataAccess notificationDataAccess) : INotificationBusiness
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan IdempotencyWindow = TimeSpan.FromHours(24);

    public NotificationBusiness(
        INotificationTemplateRepository templateRepository,
        INotificationRequestRepository requestRepository,
        IUnitOfWork unitOfWork,
        ITemplateRenderer templateRenderer,
        INotificationValidationService validationService,
        INotificationProviderFactory providerFactory,
        IClock clock,
        INotificationMetrics metrics,
        ILogger<NotificationBusiness> logger)
        : this(new NotificationDataAccess(
            templateRepository,
            requestRepository,
            unitOfWork,
            templateRenderer,
            validationService,
            providerFactory,
            clock,
            metrics,
            logger))
    {
    }

    public async Task<SendNotificationResponseDto> SendAsync(SendNotificationRequestDto request, CancellationToken cancellationToken)
    {
        notificationDataAccess.Metrics.RequestReceived();
        var stopwatch = Stopwatch.StartNew();
        var validation = notificationDataAccess.ValidationService.ValidateSendRequest(request);

        if (!validation.IsValid)
        {
            notificationDataAccess.Metrics.SendCompleted(NotificationRequestStatus.InvalidVariables, "none", stopwatch.Elapsed.TotalSeconds);
            return new SendNotificationResponseDto
            {
                Status = NotificationRequestStatus.InvalidVariables,
                Type = request.Type,
                TemplateId = request.TemplateId,
                Message = string.Join(" ", validation.Errors)
            };
        }

        var primaryRecipientAddress = request.Recipients.First().Address;
        if (!string.IsNullOrWhiteSpace(request.ExternalReference))
        {
            var existing = await notificationDataAccess.RequestRepository.GetByIdempotencyKeyAsync(
                request.Type,
                request.TemplateId,
                request.ExternalReference,
                primaryRecipientAddress,
                notificationDataAccess.Clock.UtcNow.Subtract(IdempotencyWindow),
                cancellationToken);

            if (existing is not null)
            {
                notificationDataAccess.Logger.LogInformation("Returning idempotent notification request {NotificationRequestId}.", existing.Id);
                return ToSendResponse(existing, "Notification request already processed.", idempotentReplay: true);
            }
        }

        var template = await notificationDataAccess.TemplateRepository.GetActiveByIdAsync(request.TemplateId, request.Type, cancellationToken);
        if (template is null)
        {
            var notFoundRequest = CreateNotificationRequest(request, null, NotificationRequestStatus.TemplateNotFound, "Template was not found or is inactive.");
            await notificationDataAccess.RequestRepository.AddAsync(notFoundRequest, cancellationToken);
            await notificationDataAccess.UnitOfWork.SaveChangesAsync(cancellationToken);
            notificationDataAccess.Metrics.SendCompleted(NotificationRequestStatus.TemplateNotFound, "none", stopwatch.Elapsed.TotalSeconds);
            notificationDataAccess.Logger.LogWarning("Template {TemplateId} for notification type {NotificationType} was not found or inactive.", request.TemplateId, request.Type);
            return ToSendResponse(notFoundRequest, $"Template {request.TemplateId} was not found or is inactive for {request.Type}.");
        }

        RenderedNotification renderedNotification;
        try
        {
            renderedNotification = notificationDataAccess.TemplateRenderer.Render(template, request);
        }
        catch (TemplateRenderingException ex)
        {
            var invalidRequest = CreateNotificationRequest(request, template, NotificationRequestStatus.InvalidVariables, "Required variables are missing.");
            invalidRequest.Events.Add(CreateEvent(invalidRequest.Id, NotificationEventType.ValidationFailed, string.Join(",", ex.MissingVariables), null));
            await notificationDataAccess.RequestRepository.AddAsync(invalidRequest, cancellationToken);
            await notificationDataAccess.UnitOfWork.SaveChangesAsync(cancellationToken);
            notificationDataAccess.Metrics.SendCompleted(NotificationRequestStatus.InvalidVariables, "none", stopwatch.Elapsed.TotalSeconds);
            return ToSendResponse(invalidRequest, "Required variables are missing.");
        }

        var notificationRequest = CreateNotificationRequest(request, template, NotificationRequestStatus.PendingSend, null);
        notificationRequest.SubjectRendered = renderedNotification.Subject;
        notificationRequest.Events.Add(CreateEvent(notificationRequest.Id, NotificationEventType.Created, "Notification request created.", null));
        notificationRequest.Events.Add(CreateEvent(notificationRequest.Id, NotificationEventType.Rendered, "Notification rendered.", null));

        await notificationDataAccess.RequestRepository.AddAsync(notificationRequest, cancellationToken);
        await notificationDataAccess.UnitOfWork.SaveChangesAsync(cancellationToken);

        SelectedNotificationProvider? selectedProvider = null;
        NotificationSendResult sendResult;
        var startedAtUtc = notificationDataAccess.Clock.UtcNow;

        try
        {
            selectedProvider = await notificationDataAccess.ProviderFactory.GetActiveProviderAsync(request.Type, cancellationToken);
            notificationRequest.Events.Add(CreateEvent(notificationRequest.Id, NotificationEventType.SendStarted, selectedProvider.Provider.Name, null));
            notificationDataAccess.Logger.LogInformation("Sending notification request {NotificationRequestId} using provider {Provider}.", notificationRequest.Id, selectedProvider.Provider.Name);
            sendResult = await selectedProvider.Sender.SendAsync(renderedNotification, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            notificationDataAccess.Logger.LogError(ex, "Unexpected notification provider failure for request {NotificationRequestId}.", notificationRequest.Id);
            sendResult = NotificationSendResult.Fail("ProviderFailure", "Notification provider failed unexpectedly.");
        }

        var finalStatus = sendResult.Success ? NotificationRequestStatus.Sent : NotificationRequestStatus.Failed;
        var safeError = sendResult.Success ? null : Sanitize(sendResult.ErrorMessage ?? "Notification send failed.");
        notificationRequest.Status = finalStatus;
        notificationRequest.ErrorMessage = safeError;
        notificationRequest.UpdatedAtUtc = notificationDataAccess.Clock.UtcNow;
        notificationRequest.SentAtUtc = sendResult.Success ? notificationDataAccess.Clock.UtcNow : null;

        notificationRequest.Attempts.Add(new NotificationSendAttempt
        {
            Id = Guid.NewGuid(),
            NotificationRequestId = notificationRequest.Id,
            NotificationProviderId = selectedProvider?.Provider.Id,
            AttemptNumber = 1,
            Success = sendResult.Success,
            ProviderMessageId = sendResult.ProviderMessageId,
            ErrorCode = sendResult.ErrorCode,
            ErrorMessage = safeError,
            StartedAtUtc = startedAtUtc,
            FinishedAtUtc = notificationDataAccess.Clock.UtcNow
        });

        notificationRequest.Events.Add(CreateEvent(
            notificationRequest.Id,
            sendResult.Success ? NotificationEventType.SendSucceeded : NotificationEventType.SendFailed,
            sendResult.Success ? "Notification sent successfully." : safeError,
            null));

        await notificationDataAccess.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (!sendResult.Success && selectedProvider is not null)
        {
            notificationDataAccess.Metrics.ProviderFailure(selectedProvider.Provider.Name);
        }

        notificationDataAccess.Metrics.SendCompleted(finalStatus, selectedProvider?.Provider.Name ?? "none", stopwatch.Elapsed.TotalSeconds);
        return ToSendResponse(notificationRequest, sendResult.Success ? "Notification sent successfully." : "Notification send failed.");
    }

    public async Task<NotificationStatusDto?> GetStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await notificationDataAccess.RequestRepository.GetByIdAsync(id, cancellationToken);
        return request is null ? null : ToStatusDto(request);
    }

    public async Task<NotificationTemplateDto> CreateTemplateAsync(NotificationTemplateDto template, CancellationToken cancellationToken)
    {
        var validation = notificationDataAccess.ValidationService.ValidateTemplate(template);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        if (await notificationDataAccess.TemplateRepository.CodeVersionExistsAsync(template.Type, template.Code, template.Version, null, cancellationToken))
        {
            throw new InvalidOperationException("Template code and version already exist.");
        }

        var entity = ToTemplateEntity(template);
        await notificationDataAccess.TemplateRepository.AddAsync(entity, cancellationToken);
        await notificationDataAccess.UnitOfWork.SaveChangesAsync(cancellationToken);
        return ToTemplateDto(entity);
    }

    public async Task<NotificationTemplateDto?> UpdateTemplateAsync(Guid id, NotificationTemplateDto template, CancellationToken cancellationToken)
    {   
        var entity = await notificationDataAccess.TemplateRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var validation = notificationDataAccess.ValidationService.ValidateTemplate(template);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(" ", validation.Errors));
        }

        if (await notificationDataAccess.TemplateRepository.CodeVersionExistsAsync(template.Type, template.Code, template.Version, id, cancellationToken))
        {
            throw new InvalidOperationException("Template code and version already exist.");
        }

        entity.Type = template.Type;
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
        entity.UpdatedAtUtc = notificationDataAccess.Clock.UtcNow;

        notificationDataAccess.TemplateRepository.Update(entity);
        await notificationDataAccess.UnitOfWork.SaveChangesAsync(cancellationToken);
        return ToTemplateDto(entity);
    }

    public async Task<bool> SetTemplateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var template = await notificationDataAccess.TemplateRepository.GetByIdAsync(id, cancellationToken);
        if (template is null)
        {
            return false;
        }

        template.IsActive = isActive;
        template.UpdatedAtUtc = notificationDataAccess.Clock.UtcNow;
        notificationDataAccess.TemplateRepository.Update(template);
        await notificationDataAccess.UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<NotificationTemplateDto?> GetTemplateByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var template = await notificationDataAccess.TemplateRepository.GetByCodeAsync(code, cancellationToken);
        return template is null ? null : ToTemplateDto(template);
    }

    private NotificationRequest CreateNotificationRequest(SendNotificationRequestDto request, NotificationTemplate? template, NotificationRequestStatus status, string? errorMessage)
    {
        var now = notificationDataAccess.Clock.UtcNow;
        var entity = new NotificationRequest
        {
            Id = Guid.NewGuid(),
            TemplateId = template?.Id,
            Type = request.Type,
            TemplateCode = template?.Code ?? string.Empty,
            ExternalReference = request.ExternalReference,
            Source = request.Source,
            Status = status,
            Priority = request.Priority,
            VariablesJson = JsonSerializer.Serialize(request.Variables, JsonOptions),
            MetadataJson = request.Metadata is null ? null : JsonSerializer.Serialize(request.Metadata, JsonOptions),
            ErrorMessage = errorMessage,
            CreatedAtUtc = now,
            UpdatedAtUtc = status is NotificationRequestStatus.TemplateNotFound or NotificationRequestStatus.InvalidVariables ? now : null
        };

        foreach (var recipient in request.Recipients)
        {
            entity.Recipients.Add(new NotificationRecipient
            {
                Id = Guid.NewGuid(),
                NotificationRequestId = entity.Id,
                Address = recipient.Address,
                Name = recipient.Name,
                Role = recipient.Role,
                CreatedAtUtc = now
            });
        }

        return entity;
    }

    private NotificationEvent CreateEvent(Guid requestId, NotificationEventType eventType, string? description, string? metadataJson)
        => new()
        {
            Id = Guid.NewGuid(),
            NotificationRequestId = requestId,
            EventType = eventType,
            Description = Sanitize(description),
            MetadataJson = metadataJson,
            CreatedAtUtc = notificationDataAccess.Clock.UtcNow
        };

    private static string? Sanitize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Length <= 1000 ? value : value[..1000];
    }

    private static SendNotificationResponseDto ToSendResponse(NotificationRequest request, string message, bool idempotentReplay = false)
        => new()
        {
            Id = request.Id,
            Type = request.Type,
            TemplateId = request.TemplateId,
            Status = request.Status,
            TemplateCode = request.TemplateCode,
            Message = message,
            IdempotentReplay = idempotentReplay
        };

    private static NotificationStatusDto ToStatusDto(NotificationRequest request)
        => new()
        {
            Id = request.Id,
            Type = request.Type,
            TemplateId = request.TemplateId,
            TemplateCode = request.TemplateCode,
            Status = request.Status,
            ExternalReference = request.ExternalReference,
            Source = request.Source,
            SubjectRendered = request.SubjectRendered,
            ErrorMessage = request.ErrorMessage,
            CreatedAtUtc = request.CreatedAtUtc,
            SentAtUtc = request.SentAtUtc
        };

    private NotificationTemplate ToTemplateEntity(NotificationTemplateDto template)
        => new()
        {
            Id = template.Id ?? Guid.NewGuid(),
            Type = template.Type,
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
            CreatedAtUtc = notificationDataAccess.Clock.UtcNow
        };

    private static NotificationTemplateDto ToTemplateDto(NotificationTemplate template)
        => new()
        {
            Id = template.Id,
            Type = template.Type,
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
