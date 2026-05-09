using System.Text.Json;
using Kuva.Notifications.Entities.Entities;

namespace Kuva.Notifications.Tests.Builders;

public sealed class NotificationTemplateBuilder
{
    private readonly NotificationTemplate _template = new()
    {
        Id = Kuva.Notifications.Repository.Context.NotificationsDbContext.OrderReceivedTemplateId,
        Code = "ORDER_RECEIVED",
        Name = "Pedido recebido",
        SubjectTemplate = "Pedido {{orderNumber}} recebido",
        HtmlBodyTemplate = "<h1>Ola {{customerName}}</h1>",
        RequiredVariablesJson = JsonSerializer.Serialize(new[] { "orderNumber", "customerName" }),
        Language = "pt-BR",
        Version = 1,
        IsActive = true,
        CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    public NotificationTemplateBuilder WithCode(string code)
    {
        _template.Code = code;
        return this;
    }

    public NotificationTemplateBuilder Inactive()
    {
        _template.IsActive = false;
        return this;
    }

    public NotificationTemplate Build() => _template;
}
