using FluentAssertions;
using Kuva.Email.Business.Models;
using Kuva.Email.Business.Services;
using Kuva.Email.Tests.Builders;

namespace Kuva.Email.Tests.Business;

[TestFixture]
public sealed class TemplateRendererTests
{
    private TemplateRenderer _sut = null!;

    [SetUp]
    public void SetUp() => _sut = new TemplateRenderer();

    [Test]
    public void Render_WhenVariablesAreProvided_ShouldReplaceSubjectAndHtml()
    {
        var template = new EmailTemplateBuilder().Build();
        var request = new SendEmailRequestDtoBuilder().Build();

        var result = _sut.Render(template, request);

        result.Subject.Should().Be("Pedido 123 recebido");
        result.HtmlBody.Should().Be("<h1>Ola Cliente</h1>");
    }

    [Test]
    public void Render_WhenRequiredVariableIsMissing_ShouldFail()
    {
        var template = new EmailTemplateBuilder().Build();
        var request = new SendEmailRequestDtoBuilder().WithoutVariable("customerName").Build();

        var act = () => _sut.Render(template, request);

        act.Should().Throw<TemplateRenderingException>()
            .Where(x => x.MissingVariables.Contains("customerName"));
    }
}
