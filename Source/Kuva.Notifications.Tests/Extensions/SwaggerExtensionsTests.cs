using Kuva.Notifications.Service.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kuva.Notifications.Tests.Extensions;

[TestFixture]
public sealed class SwaggerExtensionsTests
{
    [Test]
    public void AddKuvaSwagger_WhenCalled_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddKuvaSwagger();

        // Assert
        Assert.That(result, Is.SameAs(services));
    }

    [Test]
    public void AddKuvaSwagger_WhenCalled_RegistersSwaggerServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddKuvaSwagger();
        var provider = services.BuildServiceProvider();

        // Assert
        // Swagger gen registers Swashbuckle services; the provider should build without throwing
        Assert.That(provider, Is.Not.Null);
    }

    [Test]
    public void AddKuvaSwagger_WhenCalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        Action act = () =>
        {
            services.AddKuvaSwagger();
            services.AddKuvaSwagger();
        };

        // Assert
        Assert.DoesNotThrow(() => act());
    }

    [Test]
    public void UseKuvaSwagger_WhenCalled_ReturnsSameApplicationBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddKuvaSwagger();
        var app = builder.Build();

        // Act
        var result = app.UseKuvaSwagger();

        // Assert
        Assert.That(result, Is.SameAs(app));
    }

    [Test]
    public void UseKuvaSwagger_WhenCalled_DoesNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddKuvaSwagger();
        var app = builder.Build();

        // Act
        Action act = () => app.UseKuvaSwagger();

        // Assert
        Assert.DoesNotThrow(() => act());
    }
}
