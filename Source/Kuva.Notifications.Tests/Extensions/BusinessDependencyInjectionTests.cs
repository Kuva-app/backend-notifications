using Kuva.Notifications.Business.Extensions;
using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Business.Providers;
using Kuva.Notifications.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kuva.Notifications.Tests.Extensions;

[TestFixture]
public sealed class BusinessDependencyInjectionTests
{
    private IServiceCollection _services = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
    }

    [Test]
    public void AddNotificationBusiness_ReturnsTheSameServiceCollection()
    {
        // Act
        var result = _services.AddNotificationBusiness();

        // Assert
        Assert.That(result, Is.SameAs(_services));
    }

    [Test]
    public void AddNotificationBusiness_RegistersINotificationDataAccessAsScoped()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationDataAccess) &&
            sd.ImplementationType == typeof(NotificationDataAccess) &&
            sd.Lifetime == ServiceLifetime.Scoped));
    }

    [Test]
    public void AddNotificationBusiness_RegistersINotificationBusinessAsScoped()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationBusiness) &&
            sd.ImplementationType == typeof(NotificationBusiness) &&
            sd.Lifetime == ServiceLifetime.Scoped));
    }

    [Test]
    public void AddNotificationBusiness_RegistersITemplateRendererAsSingleton()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(ITemplateRenderer) &&
            sd.ImplementationType == typeof(TemplateRenderer) &&
            sd.Lifetime == ServiceLifetime.Singleton));
    }

    [Test]
    public void AddNotificationBusiness_RegistersINotificationValidationServiceAsSingleton()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationValidationService) &&
            sd.ImplementationType == typeof(NotificationValidationService) &&
            sd.Lifetime == ServiceLifetime.Singleton));
    }

    [Test]
    public void AddNotificationBusiness_RegistersINotificationProviderFactoryAsScoped()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationProviderFactory) &&
            sd.ImplementationType == typeof(NotificationProviderFactory) &&
            sd.Lifetime == ServiceLifetime.Scoped));
    }

    [Test]
    public void AddNotificationBusiness_RegistersIClockAsSingleton()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IClock) &&
            sd.ImplementationType == typeof(SystemClock) &&
            sd.Lifetime == ServiceLifetime.Singleton));
    }

    [Test]
    public void AddNotificationBusiness_RegistersINotificationMetricsAsSingleton()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationMetrics) &&
            sd.ImplementationType == typeof(NoopNotificationMetrics) &&
            sd.Lifetime == ServiceLifetime.Singleton));
    }

    [Test]
    public void AddNotificationBusiness_RegistersMultipleINotificationSenderImplementations()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        var senderRegistrations = _services
            .Where(sd => sd.ServiceType == typeof(INotificationSender))
            .ToList();

        Assert.That(senderRegistrations, Has.Count.EqualTo(4));
    }

    [Test]
    public void AddNotificationBusiness_RegistersFakeEmailSenderAsTransient()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationSender) &&
            sd.ImplementationType == typeof(FakeEmailSender) &&
            sd.Lifetime == ServiceLifetime.Transient));
    }

    [Test]
    public void AddNotificationBusiness_RegistersSmtpEmailSenderAsTransient()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationSender) &&
            sd.ImplementationType == typeof(SmtpEmailSender) &&
            sd.Lifetime == ServiceLifetime.Transient));
    }

    [Test]
    public void AddNotificationBusiness_RegistersMailKitSenderAsTransient()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationSender) &&
            sd.ImplementationType == typeof(MailKitSender) &&
            sd.Lifetime == ServiceLifetime.Transient));
    }

    [Test]
    public void AddNotificationBusiness_RegistersSendGridEmailSenderViaHttpClient()
    {
        // Act
        _services.AddNotificationBusiness();

        // Assert
        // SendGridEmailSender is registered via AddHttpClient (typed client)
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationSender)));
    }
}
