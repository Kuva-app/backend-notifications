using FluentAssertions;
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

        result.Subject.Should().Be("Pedido 123 recebido");
        result.HtmlBody.Should().Be("<h1>Ola Cliente</h1>");
    }

    [Test]
    public void Render_WhenRequiredVariableIsMissing_ShouldFail()
    {
        var template = new NotificationTemplateBuilder().Build();
        var request = new SendNotificationRequestDtoBuilder().WithoutVariable("customerName").Build();

        var act = () => _sut.Render(template, request);

        act.Should().Throw<TemplateRenderingException>()
            .Where(x => x.MissingVariables.Contains("customerName"));
    }
}
