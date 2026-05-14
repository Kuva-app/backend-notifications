using Kuva.Notifications.Repository.Extensions;
using Kuva.Notifications.Repository.Interfaces;
using Kuva.Notifications.Repository.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Kuva.Notifications.Tests.Extensions;

[TestFixture]
public sealed class RepositoryDependencyInjectionTests
{
    private IServiceCollection _services = null!;
    private Mock<IConfiguration> _configurationMock = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _configurationMock = new Mock<IConfiguration>();

        var connectionStringsSection = new Mock<IConfigurationSection>();
        connectionStringsSection.Setup(s => s.Value).Returns("Server=.;Database=Test;Trusted_Connection=True;");

        _configurationMock
            .Setup(c => c.GetSection("ConnectionStrings"))
            .Returns(connectionStringsSection.Object);

        var notificationsDatabaseSection = new Mock<IConfigurationSection>();
        notificationsDatabaseSection.Setup(s => s.Value).Returns("Server=.;Database=Test;Trusted_Connection=True;");

        connectionStringsSection
            .Setup(s => s.GetSection("NotificationsDatabase"))
            .Returns(notificationsDatabaseSection.Object);

        _configurationMock
            .Setup(c => c.GetSection("ConnectionStrings:NotificationsDatabase"))
            .Returns(notificationsDatabaseSection.Object);
    }

    [Test]
    public void AddNotificationsRepository_ReturnsTheSameServiceCollection()
    {
        // Act
        var result = _services.AddNotificationsRepository(_configurationMock.Object);

        // Assert
        Assert.That(result, Is.SameAs(_services));
    }

    [Test]
    public void AddNotificationsRepository_RegistersINotificationTemplateRepositoryAsScoped()
    {
        // Act
        _services.AddNotificationsRepository(_configurationMock.Object);

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationTemplateRepository) &&
            sd.ImplementationType == typeof(NotificationTemplateRepository) &&
            sd.Lifetime == ServiceLifetime.Scoped));
    }

    [Test]
    public void AddNotificationsRepository_RegistersINotificationRequestRepositoryAsScoped()
    {
        // Act
        _services.AddNotificationsRepository(_configurationMock.Object);

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationRequestRepository) &&
            sd.ImplementationType == typeof(NotificationRequestRepository) &&
            sd.Lifetime == ServiceLifetime.Scoped));
    }

    [Test]
    public void AddNotificationsRepository_RegistersINotificationProviderRepositoryAsScoped()
    {
        // Act
        _services.AddNotificationsRepository(_configurationMock.Object);

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationProviderRepository) &&
            sd.ImplementationType == typeof(NotificationProviderRepository) &&
            sd.Lifetime == ServiceLifetime.Scoped));
    }

    [Test]
    public void AddNotificationsRepository_RegistersIUnitOfWorkAsScoped()
    {
        // Act
        _services.AddNotificationsRepository(_configurationMock.Object);

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IUnitOfWork) &&
            sd.ImplementationType == typeof(UnitOfWork) &&
            sd.Lifetime == ServiceLifetime.Scoped));
    }
}
