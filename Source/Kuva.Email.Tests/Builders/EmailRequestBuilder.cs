using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Enums;

namespace Kuva.Email.Tests.Builders;

public sealed class EmailRequestBuilder
{
    private readonly EmailRequest _request = new()
    {
        Id = Guid.NewGuid(),
        TemplateCode = "ORDER_RECEIVED",
        ExternalReference = "order-123",
        Status = EmailRequestStatus.Sent,
        Priority = EmailPriority.Normal,
        VariablesJson = "{}",
        CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    public EmailRequestBuilder WithRecipient(string email)
    {
        _request.Recipients.Add(new EmailRecipient
        {
            Id = Guid.NewGuid(),
            EmailRequestId = _request.Id,
            Email = email,
            Type = "To",
            CreatedAtUtc = _request.CreatedAtUtc
        });
        return this;
    }

    public EmailRequest Build() => _request;
}
