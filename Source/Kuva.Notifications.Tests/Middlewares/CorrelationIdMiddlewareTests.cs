using Kuva.Notifications.Service.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kuva.Notifications.Tests.Middlewares;

[TestFixture]
public sealed class CorrelationIdMiddlewareTests
{
    private Mock<ILogger<CorrelationIdMiddleware>> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<CorrelationIdMiddleware>>();
    }

    private CorrelationIdMiddleware CreateSut(RequestDelegate next) =>
        new(next, _logger.Object);

    private static DefaultHttpContext CreateHttpContext() => new();

    [Test]
    public async Task InvokeAsync_WhenCorrelationIdHeaderPresent_ShouldUseHeaderValue()
    {
        const string correlationId = "abc123";
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;

        await sut.InvokeAsync(context);

        Assert.That(context.TraceIdentifier, Is.EqualTo(correlationId));
    }

    [Test]
    public async Task InvokeAsync_WhenCorrelationIdHeaderPresent_ShouldSetResponseHeader()
    {
        const string correlationId = "abc123";
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;

        await sut.InvokeAsync(context);

        Assert.That(context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString(), Is.EqualTo(correlationId));
    }

    [Test]
    public async Task InvokeAsync_WhenCorrelationIdHeaderMissing_ShouldGenerateNewGuid()
    {
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.TraceIdentifier, Is.Not.Null.And.Not.Empty);
        Assert.That(Guid.TryParseExact(context.TraceIdentifier, "N", out _), Is.True);
    }

    [Test]
    public async Task InvokeAsync_WhenCorrelationIdHeaderMissing_ShouldSetResponseHeaderWithGeneratedId()
    {
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(
            context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString(),
            Is.EqualTo(context.TraceIdentifier));
    }

    [Test]
    public async Task InvokeAsync_WhenCorrelationIdHeaderIsWhitespace_ShouldGenerateNewGuid()
    {
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = "   ";

        await sut.InvokeAsync(context);

        Assert.That(Guid.TryParseExact(context.TraceIdentifier, "N", out _), Is.True);
    }

    [Test]
    public async Task InvokeAsync_WhenCorrelationIdHeaderIsEmpty_ShouldGenerateNewGuid()
    {
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = string.Empty;

        await sut.InvokeAsync(context);

        Assert.That(Guid.TryParseExact(context.TraceIdentifier, "N", out _), Is.True);
    }

    [Test]
    public async Task InvokeAsync_ShouldCallNext()
    {
        var called = false;
        RequestDelegate next = _ =>
        {
            called = true;
            return Task.CompletedTask;
        };
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(called, Is.True);
    }

    [Test]
    public async Task InvokeAsync_ShouldPassSameContextToNext()
    {
        HttpContext? capturedContext = null;
        RequestDelegate next = ctx =>
        {
            capturedContext = ctx;
            return Task.CompletedTask;
        };
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(capturedContext, Is.SameAs(context));
    }

    [Test]
    public async Task InvokeAsync_WhenCorrelationIdHeaderPresent_ShouldSetTraceIdentifierToSameValueAsResponseHeader()
    {
        const string correlationId = "my-correlation-id";
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;

        await sut.InvokeAsync(context);

        Assert.That(context.TraceIdentifier, Is.EqualTo(
            context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString()));
    }
}
