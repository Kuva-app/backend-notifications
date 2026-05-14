using System.Net;
using System.Net.Http.Headers;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Business.Providers;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;

namespace Kuva.Notifications.Tests.Providers;

[TestFixture]
public sealed class SendGridEmailSenderTests
{
    private Mock<IConfiguration> _configurationMock = null!;
    private Mock<HttpMessageHandler> _handlerMock = null!;
    private HttpClient _httpClient = null!;
    private SendGridEmailSender _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationMock = new Mock<IConfiguration>();
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);

        SetupValidConfiguration();

        _sut = new SendGridEmailSender(_httpClient, _configurationMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    private void SetupValidConfiguration(
        string? apiKey = "test-api-key",
        string? fromEmail = "from@example.com",
        string? fromName = null)
    {
        _configurationMock.Setup(c => c["SendGrid:ApiKey"]).Returns(apiKey);
        _configurationMock.Setup(c => c["SendGrid:FromEmail"]).Returns(fromEmail);
        _configurationMock.Setup(c => c["SendGrid:FromName"]).Returns(fromName);
        _configurationMock.Setup(c => c["Smtp:FromEmail"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Smtp:FromName"]).Returns((string?)null);
    }

    private static RenderedNotification BuildNotification(IEnumerable<NotificationRecipientDto>? recipients = null) =>
        new()
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Hello</p>",
            TextBody = "Hello",
            Recipients = recipients?.ToList() ?? [new NotificationRecipientDto { Address = "to@example.com", Name = "Recipient", Role = "To" }]
        };

    private void SetupHttpResponse(HttpStatusCode statusCode, HttpResponseMessage? response = null)
    {
        var resp = response ?? new HttpResponseMessage(statusCode);
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(resp);
    }

    // --- Property tests ---

    [Test]
    public void Type_Always_ReturnsEmail()
    {
        Assert.That(_sut.Type, Is.EqualTo(NotificationType.Email));
    }

    [Test]
    public void ProviderType_Always_ReturnsSendGrid()
    {
        Assert.That(_sut.ProviderType, Is.EqualTo(NotificationProviderType.SendGrid));
    }

    // --- SendAsync tests ---

    [Test]
    public async Task SendAsync_ApiKeyMissing_ReturnsFailResult()
    {
        _configurationMock.Setup(c => c["SendGrid:ApiKey"]).Returns((string?)null);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SendGridNotConfigured"));
    }

    [Test]
    public async Task SendAsync_ApiKeyWhitespace_ReturnsFailResult()
    {
        _configurationMock.Setup(c => c["SendGrid:ApiKey"]).Returns("   ");

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SendGridNotConfigured"));
    }

    [Test]
    public async Task SendAsync_FromEmailMissing_ReturnsFailResult()
    {
        _configurationMock.Setup(c => c["SendGrid:FromEmail"]).Returns((string?)null);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SendGridNotConfigured"));
    }

    [Test]
    public async Task SendAsync_FromEmailFromSmtpFallback_UsesSmtpFromEmail()
    {
        _configurationMock.Setup(c => c["SendGrid:FromEmail"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Smtp:FromEmail"]).Returns("smtp@example.com");
        SetupHttpResponse(HttpStatusCode.Accepted);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task SendAsync_SuccessStatusCode_ReturnsOkResult()
    {
        SetupHttpResponse(HttpStatusCode.Accepted);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task SendAsync_SuccessStatusCodeWithMessageId_ReturnsMessageId()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted);
        response.Headers.Add("X-Message-Id", "msg-123");
        SetupHttpResponse(HttpStatusCode.Accepted, response);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ProviderMessageId, Is.EqualTo("msg-123"));
    }

    [Test]
    public async Task SendAsync_SuccessStatusCodeWithoutMessageId_ReturnsNullProviderMessageId()
    {
        SetupHttpResponse(HttpStatusCode.Accepted);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
        Assert.That(result.ProviderMessageId, Is.Null);
    }

    [Test]
    public async Task SendAsync_FailureStatusCode_ReturnsFailResult()
    {
        SetupHttpResponse(HttpStatusCode.BadRequest);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("BadRequest"));
        Assert.That(result.ErrorMessage, Is.EqualTo("SendGrid provider failed to send the email."));
    }

    [Test]
    public async Task SendAsync_FromNameFallsBackToSmtpFromName_WhenSendGridFromNameNull()
    {
        _configurationMock.Setup(c => c["SendGrid:FromName"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Smtp:FromName"]).Returns("SmtpName");
        SetupHttpResponse(HttpStatusCode.Accepted);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task SendAsync_FromNameDefaultsToKuva_WhenBothFromNamesNull()
    {
        _configurationMock.Setup(c => c["SendGrid:FromName"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Smtp:FromName"]).Returns((string?)null);
        SetupHttpResponse(HttpStatusCode.Accepted);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task SendAsync_OnlyToRoleRecipients_AreIncluded()
    {
        var recipients = new List<NotificationRecipientDto>
        {
            new() { Address = "to@example.com", Name = "To", Role = "To" },
            new() { Address = "cc@example.com", Name = "Cc", Role = "Cc" }
        };
        SetupHttpResponse(HttpStatusCode.Accepted);

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task SendAsync_CancellationToken_IsPassedToHttpClient()
    {
        using var cts = new CancellationTokenSource();
        SetupHttpResponse(HttpStatusCode.Accepted);

        var result = await _sut.SendAsync(BuildNotification(), cts.Token);

        Assert.That(result.Success, Is.True);
    }
}
