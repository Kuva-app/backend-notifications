using Kuva.Notifications.Business.Interfaces;
using Kuva.Notifications.Entities.Enums;
using Kuva.Notifications.Repository.Context;
using Kuva.Notifications.Service.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Extensions;

[TestFixture]
public sealed class ObservabilityExtensionsTests
{
    [Test]
    public void AddKuvaObservability_ReturnsTheSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddKuvaObservability();

        // Assert
        Assert.That(result, Is.SameAs(services));
    }

    [Test]
    public void AddKuvaObservability_RegistersINotificationMetricsAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddKuvaObservability();

        // Assert
        Assert.That(services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(INotificationMetrics) &&
            sd.ImplementationType == typeof(PrometheusNotificationMetrics) &&
            sd.Lifetime == ServiceLifetime.Singleton));
    }

    [Test]
    public void AddKuvaObservability_RegistersHealthChecks()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddKuvaObservability();

        // Assert
        Assert.That(services, Has.Some.Matches<ServiceDescriptor>(sd => sd.ServiceType.Name.Contains("HealthCheck") || sd.ServiceType.Name.Contains("IHealthCheck")));
    }

    [Test]
    public void AddKuvaObservability_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddKuvaObservability();

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void MapKuvaObservability_ReturnsTheSameEndpointRouteBuilder()
    {
        // Arrange
        var app = CreateWebApplication();

        // Act
        var result = app.MapKuvaObservability();

        // Assert
        Assert.That(result, Is.SameAs(app));
    }

    [Test]
    public void MapKuvaObservability_DoesNotThrow()
    {
        // Arrange
        var app = CreateWebApplication();

        // Act
        var act = () => app.MapKuvaObservability();

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    private static WebApplication CreateWebApplication()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddHealthChecks();
        return builder.Build();
    }
}

[TestFixture]
public sealed class PrometheusNotificationMetricsTests
{
    private PrometheusNotificationMetrics _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new PrometheusNotificationMetrics();
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
    public void SendCompleted_WithStatusSent_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.Sent, "provider", 0.5);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WithStatusTemplateNotFound_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.TemplateNotFound, "provider", 0.1);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WithStatusInvalidVariables_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.InvalidVariables, "provider", 0.2);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WithStatusFailed_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.Failed, "provider", 1.0);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WithUnhandledStatus_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.Created, "provider", 0.0);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WithZeroDuration_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.Sent, "provider", 0.0);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_WithLargeDuration_DoesNotThrow()
    {
        // Act
        var act = () => _sut.SendCompleted(NotificationRequestStatus.Sent, "provider", 9999.9);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void ProviderFailure_WhenCalled_DoesNotThrow()
    {
        // Act
        var act = () => _sut.ProviderFailure("some-provider");

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void ProviderFailure_WithDifferentProviderNames_DoesNotThrow(
        [Values("smtp", "sendgrid", "mailkit", "fake")] string providerName)
    {
        // Act
        var act = () => _sut.ProviderFailure(providerName);

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void SendCompleted_ObservesDurationForAllStatusesWithLabels_DoesNotThrow(
        [Values(
            NotificationRequestStatus.Sent,
            NotificationRequestStatus.TemplateNotFound,
            NotificationRequestStatus.InvalidVariables,
            NotificationRequestStatus.Failed,
            NotificationRequestStatus.Created)]
        NotificationRequestStatus status)
    {
        // Act
        var act = () => _sut.SendCompleted(status, "test-provider", 0.123);

        // Assert
        Assert.DoesNotThrow(() => act());
    }
}

[TestFixture]
public sealed class SqlServerHealthCheckTests
{
    private static NotificationsDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new NotificationsDbContext(options);
    }

    private static NotificationsDbContext CreateUnreachableSqlContext()
    {
        var options = new DbContextOptionsBuilder<NotificationsDbContext>()
            .UseSqlServer("Server=127.0.0.1,19999;Database=NonExistent;User Id=sa;Password=x;Connect Timeout=1;TrustServerCertificate=True;")
            .Options;
        return new NotificationsDbContext(options);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCanConnectReturnsTrue_ReturnsHealthy()
    {
        // Arrange
        await using var dbContext = CreateInMemoryContext();
        var sut = new SqlServerHealthCheck(dbContext);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("sql", sut, HealthStatus.Unhealthy, null)
        };

        // Act
        var result = await sut.CheckHealthAsync(context);

        // Assert
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
    }

    [Test]
    public async Task CheckHealthAsync_WhenCancellationTokenIsAlreadyCancelled_ReturnsUnhealthyWithException()
    {
        // Arrange
        await using var dbContext = CreateUnreachableSqlContext();
        var sut = new SqlServerHealthCheck(dbContext);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("sql", sut, HealthStatus.Unhealthy, null)
        };
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var result = await sut.CheckHealthAsync(context, cts.Token);

        // Assert
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
        Assert.That(result.Exception, Is.Not.Null);
    }

    [Test]
    public async Task CheckHealthAsync_WhenCanConnectThrows_ReturnsUnhealthyWithExceptionMessage()
    {
        // Arrange
        await using var dbContext = CreateUnreachableSqlContext();
        var sut = new SqlServerHealthCheck(dbContext);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("sql", sut, HealthStatus.Unhealthy, null)
        };

        // Act
        var result = await sut.CheckHealthAsync(context);

        // Assert
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
        Assert.That(result.Description, Is.EqualTo("SQL Server is not reachable."));
    }
}
