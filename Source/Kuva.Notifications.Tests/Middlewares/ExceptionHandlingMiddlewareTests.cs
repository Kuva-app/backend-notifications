using Kuva.Notifications.Service.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kuva.Notifications.Tests.Middlewares;

[TestFixture]
public sealed class ExceptionHandlingMiddlewareTests
{
    private Mock<ILogger<ExceptionHandlingMiddleware>> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
    }

    private ExceptionHandlingMiddleware CreateSut(RequestDelegate next) =>
        new(next, _logger.Object);

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    [Test]
    public async Task InvokeAsync_WhenNoException_ShouldCallNext()
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
    public async Task InvokeAsync_WhenNoException_ShouldNotAlterStatusCode()
    {
        RequestDelegate next = _ => Task.CompletedTask;
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task InvokeAsync_WhenOperationCanceledAndRequestAborted_ShouldReturn499()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        RequestDelegate next = _ => throw new OperationCanceledException(cts.Token);
        var sut = CreateSut(next);
        var context = CreateHttpContext();
        context.RequestAborted = cts.Token;

        await sut.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(499));
    }

    [Test]
    public async Task InvokeAsync_WhenOperationCanceledButRequestNotAborted_ShouldReturn500()
    {
        RequestDelegate next = _ => throw new OperationCanceledException();
        var sut = CreateSut(next);
        var context = CreateHttpContext();
        // RequestAborted is not cancelled

        await sut.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task InvokeAsync_WhenArgumentException_ShouldReturn400()
    {
        RequestDelegate next = _ => throw new ArgumentException("bad arg");
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
    }

    [Test]
    public async Task InvokeAsync_WhenArgumentException_ShouldWriteProblemJson()
    {
        RequestDelegate next = _ => throw new ArgumentException("bad arg");
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.Response.ContentType, Does.StartWith("application/json"));
    }

    [Test]
    public async Task InvokeAsync_WhenInvalidOperationException_ShouldReturn409()
    {
        RequestDelegate next = _ => throw new InvalidOperationException("conflict");
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
    }

    [Test]
    public async Task InvokeAsync_WhenInvalidOperationException_ShouldWriteProblemJson()
    {
        RequestDelegate next = _ => throw new InvalidOperationException("conflict");
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.Response.ContentType, Does.StartWith("application/json"));
    }

    [Test]
    public async Task InvokeAsync_WhenUnhandledException_ShouldReturn500()
    {
        RequestDelegate next = _ => throw new Exception("boom");
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }

    [Test]
    public async Task InvokeAsync_WhenUnhandledException_ShouldLogError()
    {
        var exception = new Exception("boom");
        RequestDelegate next = _ => throw exception;
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task InvokeAsync_WhenUnhandledException_ShouldWriteProblemJson()
    {
        RequestDelegate next = _ => throw new Exception("boom");
        var sut = CreateSut(next);
        var context = CreateHttpContext();

        await sut.InvokeAsync(context);

        Assert.That(context.Response.ContentType, Does.StartWith("application/json"));
    }
}
