using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;

namespace Kuva.Notifications.Tests.Builders;

public sealed class NotificationRequestBuilder
{
    private readonly NotificationRequest _request = new()
    {
        Id = Guid.NewGuid(),
        TemplateId = Kuva.Notifications.Repository.Context.NotificationsDbContext.OrderReceivedTemplateId,
        TemplateCode = "ORDER_RECEIVED",
        ExternalReference = "order-123",
        Status = NotificationRequestStatus.Sent,
        Priority = NotificationPriority.Normal,
        VariablesJson = "{}",
        CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    public NotificationRequestBuilder WithRecipient(string email)
    {
        _request.Recipients.Add(new NotificationRecipient
        {
            Id = Guid.NewGuid(),
            NotificationRequestId = _request.Id,
            Address = email,
            Role = "To",
            CreatedAtUtc = _request.CreatedAtUtc
        });
        return this;
    }

    public NotificationRequest Build() => _request;
}
