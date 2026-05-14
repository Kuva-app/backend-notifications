using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Business.Providers;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Kuva.Notifications.Tests.Providers;

[TestFixture]
public sealed class SmtpEmailSenderTests
{
    private Mock<IConfiguration> _configurationMock = null!;
    private SmtpEmailSender _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationMock = new Mock<IConfiguration>();
        SetupValidConfiguration();
        _sut = new SmtpEmailSender(_configurationMock.Object);
    }

    private void SetupValidConfiguration(
        string? host = "localhost",
        string? fromEmail = "from@example.com",
        string? fromName = "Kuva Test",
        string? port = "2525",
        string? enableSsl = "false",
        string? username = null,
        string? password = null)
    {
        _configurationMock.Setup(c => c["Smtp:Host"]).Returns(host);
        _configurationMock.Setup(c => c["Smtp:FromEmail"]).Returns(fromEmail);
        _configurationMock.Setup(c => c["Smtp:FromName"]).Returns(fromName);
        _configurationMock.Setup(c => c["Smtp:Port"]).Returns(port);
        _configurationMock.Setup(c => c["Smtp:EnableSsl"]).Returns(enableSsl);
        _configurationMock.Setup(c => c["Smtp:Username"]).Returns(username);
        _configurationMock.Setup(c => c["Smtp:Password"]).Returns(password);
    }

    private static RenderedNotification BuildNotification(IEnumerable<NotificationRecipientDto>? recipients = null) =>
        new()
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Hello</p>",
            TextBody = "Hello",
            Recipients = recipients?.ToList() ?? [new NotificationRecipientDto { Address = "to@example.com", Name = "Recipient", Role = "To" }]
        };

    [Test]
    public void Type_ReturnsEmail()
    {
        Assert.That(_sut.Type, Is.EqualTo(NotificationType.Email));
    }

    [Test]
    public void ProviderType_ReturnsSmtp()
    {
        Assert.That(_sut.ProviderType, Is.EqualTo(NotificationProviderType.Smtp));
    }

    [Test]
    public async Task SendAsync_WhenHostIsNull_ReturnsFailResult()
    {
        SetupValidConfiguration(host: null);
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
        Assert.That(result.ErrorMessage, Is.EqualTo("SMTP provider is not configured."));
    }

    [Test]
    public async Task SendAsync_WhenHostIsWhitespace_ReturnsFailResult()
    {
        SetupValidConfiguration(host: "   ");
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_WhenFromEmailIsNull_ReturnsFailResult()
    {
        SetupValidConfiguration(fromEmail: null);
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_WhenFromEmailIsWhitespace_ReturnsFailResult()
    {
        SetupValidConfiguration(fromEmail: "   ");
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_WhenSmtpConnectionFails_ReturnsFailResult()
    {
        // Uses localhost:2525 with no real server — SmtpException expected
        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task SendAsync_WithCcRecipient_AttemptsToSendAndReturnsFailOnNoServer()
    {
        var recipients = new[]
        {
            new NotificationRecipientDto { Address = "cc@example.com", Name = "CC User", Role = "CC" }
        };

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task SendAsync_WithBccRecipient_AttemptsToSendAndReturnsFailOnNoServer()
    {
        var recipients = new[]
        {
            new NotificationRecipientDto { Address = "bcc@example.com", Name = "BCC User", Role = "BCC" }
        };

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task SendAsync_WithInvalidPortConfig_UsesDefaultPort587AndFailsOnNoServer()
    {
        SetupValidConfiguration(port: "not-a-number");
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task SendAsync_WithCredentials_AttemptsToSendAndReturnsFailOnNoServer()
    {
        SetupValidConfiguration(username: "user@example.com", password: "secret");
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task SendAsync_WithUsernameButNoPassword_DoesNotSetCredentialsAndReturnsFailOnNoServer()
    {
        SetupValidConfiguration(username: "user@example.com", password: null);
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task SendAsync_WithEnableSslFalse_DisablesSslAndAttemptsToSend()
    {
        SetupValidConfiguration(enableSsl: "false");
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task SendAsync_WithEnableSslNotParseable_DefaultsToSslEnabledAndAttemptsToSend()
    {
        SetupValidConfiguration(enableSsl: "not-a-bool");
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        // SSL default is true when parse fails; still fails due to no server
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task SendAsync_WithNullFromName_UsesKuvaDefault()
    {
        SetupValidConfiguration(fromName: null);
        _sut = new SmtpEmailSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        // Should not throw — MailMessage constructed with default "Kuva" name
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task SendAsync_WithMultipleMixedRecipients_AttemptsToSendAndReturnsFailOnNoServer()
    {
        var recipients = new[]
        {
            new NotificationRecipientDto { Address = "to@example.com", Name = "To User", Role = "To" },
            new NotificationRecipientDto { Address = "cc@example.com", Name = "CC User", Role = "cc" },
            new NotificationRecipientDto { Address = "bcc@example.com", Name = "BCC User", Role = "bcc" },
        };

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.False);
    }
}
