using Kuva.Notifications.Service.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kuva.Notifications.Tests.Extensions;

[TestFixture]
public sealed class ApplicationBuilderExtensionsTests
{
    private static WebApplication CreateApp(string environmentName)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = environmentName
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddRateLimiter(_ => { });
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();

        return builder.Build();
    }

    [Test]
    public void UseKuvaNotificationsPipeline_ReturnsSameWebApplicationInstance()
    {
        // Arrange
        var app = CreateApp("Development");

        // Act
        var result = app.UseKuvaNotificationsPipeline();

        // Assert
        Assert.That(result, Is.SameAs(app));
    }

    [Test]
    public void UseKuvaNotificationsPipeline_DevelopmentEnvironment_DoesNotThrow()
    {
        // Arrange
        var app = CreateApp("Development");

        // Act
        var act = () => app.UseKuvaNotificationsPipeline();

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void UseKuvaNotificationsPipeline_ProductionEnvironment_DoesNotThrow()
    {
        // Arrange
        var app = CreateApp("Production");

        // Act
        var act = () => app.UseKuvaNotificationsPipeline();

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void UseKuvaNotificationsPipeline_StagingEnvironment_DoesNotThrow()
    {
        // Arrange
        var app = CreateApp("Staging");

        // Act
        var act = () => app.UseKuvaNotificationsPipeline();

        // Assert
        Assert.DoesNotThrow(() => act());
    }
}
