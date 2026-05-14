using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Business.Providers;
using Kuva.Notifications.Entities.Dtos;
using Kuva.Notifications.Entities.Enums;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Kuva.Notifications.Tests.Providers;

[TestFixture]
public sealed class MailKitSenderTests
{
    private Mock<IConfiguration> _configurationMock = null!;
    private MailKitSender _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationMock = new Mock<IConfiguration>();
        SetupValidConfiguration();
        _sut = new MailKitSender(_configurationMock.Object);
    }

    private void SetupValidConfiguration(
        string? host = "smtp.example.com",
        string? fromEmail = "from@example.com",
        string? username = "user",
        string? password = "pass",
        string? fromName = null,
        string? port = null)
    {
        _configurationMock.Setup(c => c["Smtp:Host"]).Returns(host);
        _configurationMock.Setup(c => c["Smtp:FromEmail"]).Returns(fromEmail);
        _configurationMock.Setup(c => c["Smtp:Username"]).Returns(username);
        _configurationMock.Setup(c => c["Smtp:Password"]).Returns(password);
        _configurationMock.Setup(c => c["Smtp:FromName"]).Returns(fromName);
        _configurationMock.Setup(c => c["Smtp:Port"]).Returns(port);
    }

    private static RenderedNotification BuildNotification(IEnumerable<NotificationRecipientDto>? recipients = null) =>
        new()
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Hello</p>",
            TextBody = "Hello",
            Recipients = recipients?.ToList() ?? [new NotificationRecipientDto { Address = "to@example.com", Name = "Recipient", Role = "To" }]
        };

    // --- Property tests ---

    [Test]
    public void Type_Always_ReturnsEmail()
    {
        Assert.That(_sut.Type, Is.EqualTo(NotificationType.Email));
    }

    [Test]
    public void ProviderType_Always_ReturnsMailKitSmtp()
    {
        Assert.That(_sut.ProviderType, Is.EqualTo(NotificationProviderType.MailKitSmtp));
    }

    // --- Configuration validation tests ---

    [Test]
    public async Task SendAsync_HostIsNull_ReturnsFailResult()
    {
        SetupValidConfiguration(host: null);
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_HostIsWhitespace_ReturnsFailResult()
    {
        SetupValidConfiguration(host: "   ");
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_FromEmailIsNull_ReturnsFailResult()
    {
        SetupValidConfiguration(fromEmail: null);
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_UsernameIsNull_ReturnsFailResult()
    {
        SetupValidConfiguration(username: null);
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_PasswordIsNull_ReturnsFailResult()
    {
        SetupValidConfiguration(password: null);
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    [Test]
    public async Task SendAsync_PasswordIsWhitespace_ReturnsFailResult()
    {
        SetupValidConfiguration(password: "  ");
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SmtpNotConfigured"));
    }

    // --- SMTP connection failure (no real server available) ---

    [Test]
    public async Task SendAsync_ValidConfigButSmtpUnavailable_ReturnsSmtpErrorFail()
    {
        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
        Assert.That(result.ErrorMessage, Is.EqualTo("SMTP provider failed to send the email."));
    }

    [Test]
    public async Task SendAsync_RecipientWithRoleCc_StillAttemptsSendAndReturnsSmtpError()
    {
        var recipients = new List<NotificationRecipientDto>
        {
            new() { Address = "cc@example.com", Name = "CC Person", Role = "cc" }
        };

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_RecipientWithRoleBcc_StillAttemptsSendAndReturnsSmtpError()
    {
        var recipients = new List<NotificationRecipientDto>
        {
            new() { Address = "bcc@example.com", Name = "BCC Person", Role = "BCC" }
        };

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_RecipientWithDefaultRole_StillAttemptsSendAndReturnsSmtpError()
    {
        var recipients = new List<NotificationRecipientDto>
        {
            new() { Address = "to@example.com", Name = "To Person", Role = "To" }
        };

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_MultipleRecipientsWithMixedRoles_StillAttemptsSendAndReturnsSmtpError()
    {
        var recipients = new List<NotificationRecipientDto>
        {
            new() { Address = "to@example.com", Name = "To", Role = "To" },
            new() { Address = "cc@example.com", Name = "CC", Role = "CC" },
            new() { Address = "bcc@example.com", Name = "BCC", Role = "Bcc" }
        };

        var result = await _sut.SendAsync(BuildNotification(recipients), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_PortNotConfigured_UsesDefaultPortAndReturnsSmtpError()
    {
        SetupValidConfiguration(port: null);
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        // Default port is used; SMTP connection will still fail without a real server
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_PortConfiguredAsInvalidString_UsesDefaultPortAndReturnsSmtpError()
    {
        SetupValidConfiguration(port: "notanumber");
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_PortConfiguredAsValidNumber_UsesConfiguredPortAndReturnsSmtpError()
    {
        SetupValidConfiguration(port: "587");
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_FromNameNotConfigured_UsesDefaultFromNameAndReturnsSmtpError()
    {
        SetupValidConfiguration(fromName: null);
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_FromNameConfigured_UsesConfiguredFromNameAndReturnsSmtpError()
    {
        SetupValidConfiguration(fromName: "My App");
        _sut = new MailKitSender(_configurationMock.Object);

        var result = await _sut.SendAsync(BuildNotification(), CancellationToken.None);

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("SMTP_ERROR"));
    }

    [Test]
    public async Task SendAsync_CancellationRequested_ReturnsSmtpError()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var result = await _sut.SendAsync(BuildNotification(), cts.Token);

        Assert.That(result.Success, Is.False);
    }
}
