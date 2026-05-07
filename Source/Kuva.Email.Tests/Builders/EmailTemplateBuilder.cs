using System.Text.Json;
using Kuva.Email.Entities.Entities;

namespace Kuva.Email.Tests.Builders;

public sealed class EmailTemplateBuilder
{
    private readonly EmailTemplate _template = new()
    {
        Id = Guid.NewGuid(),
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

    public EmailTemplateBuilder WithCode(string code)
    {
        _template.Code = code;
        return this;
    }

    public EmailTemplateBuilder Inactive()
    {
        _template.IsActive = false;
        return this;
    }

    public EmailTemplate Build() => _template;
}
