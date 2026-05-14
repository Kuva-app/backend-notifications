using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Business.Services;
using Kuva.Notifications.Tests.Builders;

namespace Kuva.Notifications.Tests.Business;

[TestFixture]
public sealed class TemplateRendererTests
{
    private TemplateRenderer _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new TemplateRenderer();

    [Test]
    public void Render_WhenVariablesAreProvided_ShouldReplaceSubjectAndHtml()
    {
        var template = new NotificationTemplateBuilder().Build();
        var request = new SendNotificationRequestDtoBuilder().Build();

        var result = _sut.Render(template, request);

        Assert.That(result.Subject, Is.EqualTo("Pedido 123 recebido"));
        Assert.That(result.HtmlBody, Is.EqualTo("<h1>Ola Cliente</h1>"));
    }

    [Test]
    public void Render_WhenRequiredVariableIsMissing_ShouldFail()
    {
        var template = new NotificationTemplateBuilder().Build();
        var request = new SendNotificationRequestDtoBuilder().WithoutVariable("customerName").Build();

        var act = () => _sut.Render(template, request);

        var ex = Assert.Throws<TemplateRenderingException>(() => act());
        Assert.That(ex!.MissingVariables, Does.Contain("customerName"));
    }

    [Test]
    public void Render_WhenTextBodyTemplateIsNull_ShouldReturnNullTextBody()
    {
        var template = new NotificationTemplateBuilder().Build();
        template.TextBodyTemplate = null;
        var request = new SendNotificationRequestDtoBuilder().Build();

        var result = _sut.Render(template, request);

        Assert.That(result.TextBody, Is.Null);
    }

    [Test]
    public void Render_WhenTextBodyTemplateIsNotNull_ShouldReplaceTextBodyVariables()
    {
        var template = new NotificationTemplateBuilder().Build();
        template.TextBodyTemplate = "Order {{orderNumber}} for {{customerName}}";
        var request = new SendNotificationRequestDtoBuilder().Build();

        var result = _sut.Render(template, request);

        Assert.That(result.TextBody, Is.EqualTo("Order 123 for Cliente"));
    }

    [Test]
    public void Render_WhenVariableValueIsWhitespace_ShouldThrowTemplateRenderingException()
    {
        var template = new NotificationTemplateBuilder().Build();
        var request = new SendNotificationRequestDtoBuilder().Build();
        request.Variables["customerName"] = "   ";

        var act = () => _sut.Render(template, request);

        var ex = Assert.Throws<TemplateRenderingException>(() => act());
        Assert.That(ex!.MissingVariables, Does.Contain("customerName"));
    }

    [Test]
    public void Render_WhenMultipleVariablesMissing_ShouldIncludeAllInException()
    {
        var template = new NotificationTemplateBuilder().Build();
        var request = new SendNotificationRequestDtoBuilder()
            .WithoutVariable("customerName")
            .WithoutVariable("orderNumber")
            .Build();

        var act = () => _sut.Render(template, request);

        var ex = Assert.Throws<TemplateRenderingException>(() => act());
        Assert.That(ex!.MissingVariables, Does.Contain("customerName"));
        Assert.That(ex.MissingVariables, Does.Contain("orderNumber"));
    }

    [Test]
    public void Render_ShouldPassRecipientsThrough()
    {
        var template = new NotificationTemplateBuilder().Build();
        var request = new SendNotificationRequestDtoBuilder().Build();

        var result = _sut.Render(template, request);

        Assert.That(result.Recipients, Is.SameAs(request.Recipients));
    }

    [Test]
    public void Render_WhenRequiredVariablesJsonIsInvalid_ShouldSucceedWithNoValidation()
    {
        var template = new NotificationTemplateBuilder().Build();
        template.RequiredVariablesJson = "not-valid-json";
        var request = new SendNotificationRequestDtoBuilder().WithoutVariable("customerName").Build();

        var result = _sut.Render(template, request);

        Assert.That(result, Is.Not.Null);
    }
}
