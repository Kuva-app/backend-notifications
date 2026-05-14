using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Models;
using Kuva.Notifications.Business.Services;
using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Interfaces;
using Moq;

namespace Kuva.Notifications.Tests.Business;

[TestFixture]
public sealed class NotificationProviderFactoryTests
{
    [Test]
    public async Task GetActiveProviderAsync_WhenFakeProviderIsActive_ShouldReturnFakeSender()
    {
        var provider = new NotificationProvider { Id = Guid.NewGuid(), Name = "Fake", Type = NotificationType.Email, ProviderType = NotificationProviderType.Fake, IsActive = true };
        var repository = new Mock<INotificationProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(NotificationType.Email, It.IsAny<CancellationToken>())).ReturnsAsync(provider);

        var fakeSender = new Mock<INotificationSender>();
        fakeSender.SetupGet(x => x.Type).Returns(NotificationType.Email);
        fakeSender.SetupGet(x => x.ProviderType).Returns(NotificationProviderType.Fake);
        var sut = new NotificationProviderFactory(repository.Object, [fakeSender.Object]);

        var result = await sut.GetActiveProviderAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result.Provider, Is.EqualTo(provider));
        Assert.That(result.Sender.ProviderType, Is.EqualTo(NotificationProviderType.Fake));
    }

    [Test]
    public async Task GetActiveProviderAsync_WhenNoProviderExists_ShouldFail()
    {
        var repository = new Mock<INotificationProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(NotificationType.Email, It.IsAny<CancellationToken>())).ReturnsAsync((NotificationProvider?)null);
        var sut = new NotificationProviderFactory(repository.Object, []);

        var act = () => sut.GetActiveProviderAsync(NotificationType.Email, CancellationToken.None);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await act());
    }

    [Test]
    public async Task GetActiveProviderAsync_WhenNoMatchingSenderRegistered_ShouldThrowInvalidOperationException()
    {
        var provider = new NotificationProvider { Id = Guid.NewGuid(), Name = "Smtp", Type = NotificationType.Email, ProviderType = NotificationProviderType.Smtp, IsActive = true };
        var repository = new Mock<INotificationProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(NotificationType.Email, It.IsAny<CancellationToken>())).ReturnsAsync(provider);

        // Register a sender for a different provider type
        var fakeSender = new Mock<INotificationSender>();
        fakeSender.SetupGet(x => x.Type).Returns(NotificationType.Email);
        fakeSender.SetupGet(x => x.ProviderType).Returns(NotificationProviderType.Fake);
        var sut = new NotificationProviderFactory(repository.Object, [fakeSender.Object]);

        var act = async () => await sut.GetActiveProviderAsync(NotificationType.Email, CancellationToken.None);

        var _ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await act());
        Assert.That(_ex!.Message, Does.Contain("is not registered"));
    }

    [Test]
    public async Task GetActiveProviderAsync_WhenSenderExistsForDifferentNotificationType_ShouldThrowInvalidOperationException()
    {
        var provider = new NotificationProvider { Id = Guid.NewGuid(), Name = "Fake", Type = NotificationType.Sms, ProviderType = NotificationProviderType.Fake, IsActive = true };
        var repository = new Mock<INotificationProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(NotificationType.Sms, It.IsAny<CancellationToken>())).ReturnsAsync(provider);

        // Register a sender for Email only, not Sms
        var fakeSender = new Mock<INotificationSender>();
        fakeSender.SetupGet(x => x.Type).Returns(NotificationType.Email);
        fakeSender.SetupGet(x => x.ProviderType).Returns(NotificationProviderType.Fake);
        var sut = new NotificationProviderFactory(repository.Object, [fakeSender.Object]);

        var act = () => sut.GetActiveProviderAsync(NotificationType.Sms, CancellationToken.None);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await act());
    }

    [Test]
    public async Task GetActiveProviderAsync_ReturnsCorrectProviderAndSender()
    {
        var provider = new NotificationProvider { Id = Guid.NewGuid(), Name = "Smtp", Type = NotificationType.Email, ProviderType = NotificationProviderType.Smtp, IsActive = true };
        var repository = new Mock<INotificationProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(NotificationType.Email, It.IsAny<CancellationToken>())).ReturnsAsync(provider);

        var smtpSender = new Mock<INotificationSender>();
        smtpSender.SetupGet(x => x.Type).Returns(NotificationType.Email);
        smtpSender.SetupGet(x => x.ProviderType).Returns(NotificationProviderType.Smtp);

        var otherSender = new Mock<INotificationSender>();
        otherSender.SetupGet(x => x.Type).Returns(NotificationType.Email);
        otherSender.SetupGet(x => x.ProviderType).Returns(NotificationProviderType.Fake);

        var sut = new NotificationProviderFactory(repository.Object, [smtpSender.Object, otherSender.Object]);

        var result = await sut.GetActiveProviderAsync(NotificationType.Email, CancellationToken.None);

        Assert.That(result.Provider, Is.EqualTo(provider));
        Assert.That(result.Sender, Is.EqualTo(smtpSender.Object));
    }
}
