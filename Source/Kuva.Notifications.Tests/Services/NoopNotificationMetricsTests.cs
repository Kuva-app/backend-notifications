using Kuva.Notifications.Business.Services;
using Kuva.Notifications.Entities.Enums;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Services;

[TestFixture]
public class NoopNotificationMetricsTests
{
    private NoopNotificationMetrics _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new NoopNotificationMetrics();
    }

    [Test]
    public void RequestReceived_WhenCalled_DoesNotThrow()
    {
        // Act
        var act = () => _sut.RequestReceived();

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WhenCalled_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.Created, "testProvider", 1.5);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WithVariousStatuses_DoesNotThrow(
        [Values(
            NotificationRequestStatus.Created,
            NotificationRequestStatus.TemplateNotFound,
            NotificationRequestStatus.InvalidVariables,
            NotificationRequestStatus.PendingSend)]
        NotificationRequestStatus status)
    {
        // Act
        var act = () => _sut.SendCompleted(status, "provider", 0.0);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void ProviderFailure_WhenCalled_DoesNotThrow()
    {
        // Act
        var act = () => _sut.ProviderFailure("testProvider");

        // Assert
        Assert.DoesNotThrow(() => act());
    }
}
