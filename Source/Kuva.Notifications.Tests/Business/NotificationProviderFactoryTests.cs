using FluentAssertions;
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

        result.Provider.Should().Be(provider);
        result.Sender.ProviderType.Should().Be(NotificationProviderType.Fake);
    }

    [Test]
    public async Task GetActiveProviderAsync_WhenNoProviderExists_ShouldFail()
    {
        var repository = new Mock<INotificationProviderRepository>();
        repository.Setup(x => x.GetActiveByPriorityAsync(NotificationType.Email, It.IsAny<CancellationToken>())).ReturnsAsync((NotificationProvider?)null);
        var sut = new NotificationProviderFactory(repository.Object, []);

        var act = () => sut.GetActiveProviderAsync(NotificationType.Email, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
