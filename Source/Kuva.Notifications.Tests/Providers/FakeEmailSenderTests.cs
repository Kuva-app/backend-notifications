using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Business.Providers;
using Kuva.Notifications.Entities.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Providers;

[TestFixture]
public sealed class FakeEmailSenderTests
{
    private Mock<ILogger<FakeEmailSender>> _loggerMock = null!;
    private FakeEmailSender _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<FakeEmailSender>>();
        _sut = new FakeEmailSender(_loggerMock.Object);
    }

    [Test]
    public void Type_Always_ReturnsEmail()
    {
        Assert.That(_sut.Type, Is.EqualTo(NotificationType.Email));
    }

    [Test]
    public void ProviderType_Always_ReturnsFake()
    {
        Assert.That(_sut.ProviderType, Is.EqualTo(NotificationProviderType.Fake));
    }

    [Test]
    public async Task SendAsync_WithNotification_ReturnsSuccessResult()
    {
        // Arrange
        var notification = new RenderedNotification
        {
            Subject = "Test Subject",
            HtmlBody = "<p>Test</p>",
            Recipients = []
        };

        // Act
        var result = await _sut.SendAsync(notification, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task SendAsync_WithNotification_ProviderMessageIdStartsWithFake()
    {
        // Arrange
        var notification = new RenderedNotification { Subject = "Subject", HtmlBody = "Body" };

        // Act
        var result = await _sut.SendAsync(notification, CancellationToken.None);

        // Assert
        Assert.That(result.ProviderMessageId, Does.StartWith("fake-"));
    }

    [Test]
    public async Task SendAsync_CalledTwice_ReturnsDifferentProviderMessageIds()
    {
        // Arrange
        var notification = new RenderedNotification { Subject = "Subject", HtmlBody = "Body" };

        // Act
        var result1 = await _sut.SendAsync(notification, CancellationToken.None);
        var result2 = await _sut.SendAsync(notification, CancellationToken.None);

        // Assert
        Assert.That(result1.ProviderMessageId, Is.Not.EqualTo(result2.ProviderMessageId));
    }

    [Test]
    public async Task SendAsync_WithRecipients_LogsInformation()
    {
        // Arrange
        var notification = new RenderedNotification
        {
            Subject = "Hello",
            HtmlBody = "<p>Body</p>",
            Recipients =
            [
                new Kuva.Notifications.Entities.Dtos.NotificationRecipientDto { Address = "a@a.com" },
                new Kuva.Notifications.Entities.Dtos.NotificationRecipientDto { Address = "b@b.com" }
            ]
        };

        // Act
        await _sut.SendAsync(notification, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("2")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
