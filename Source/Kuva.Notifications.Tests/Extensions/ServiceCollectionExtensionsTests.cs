using Kuva.Notifications.Service.Extensions;
using Kuva.Notifications.Service.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Kuva.Notifications.Tests.Extensions;

[TestFixture]
public sealed class ServiceCollectionExtensionsTests
{
    private IServiceCollection _services = null!;
    private Mock<IConfiguration> _configurationMock = null!;
    private Mock<IWebHostEnvironment> _environmentMock = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _configurationMock = new Mock<IConfiguration>();
        _environmentMock = new Mock<IWebHostEnvironment>();

        SetupConfigurationSections();
        SetupJwtConfiguration("https://authority.example.com", "test-audience");
    }

    private void SetupConfigurationSections()
    {
        foreach (var sectionName in new[] { "Notifications", "Smtp", "SendGrid", "KeyVault" })
        {
            var section = new Mock<IConfigurationSection>();
            section.Setup(s => s.Path).Returns(sectionName);
            section.Setup(s => s.Key).Returns(sectionName);
            _configurationMock.Setup(c => c.GetSection(sectionName)).Returns(section.Object);
        }

        // Setup ConnectionStrings section used by AddNotificationsRepository
        var connectionStringsSection = new Mock<IConfigurationSection>();
        connectionStringsSection.Setup(s => s.Path).Returns("ConnectionStrings");
        connectionStringsSection.Setup(s => s.Key).Returns("ConnectionStrings");
        connectionStringsSection.Setup(s => s[It.IsAny<string>()]).Returns((string?)null);
        _configurationMock.Setup(c => c.GetSection("ConnectionStrings")).Returns(connectionStringsSection.Object);
    }

    private void SetupJwtConfiguration(string authority, string audience)
    {
        _configurationMock.Setup(c => c["Jwt:Authority"]).Returns(authority);
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns(audience);
    }

    private IServiceCollection BuildServices(bool isDevelopment = false)
    {
        _environmentMock.Setup(e => e.EnvironmentName)
            .Returns(isDevelopment ? "Development" : "Production");

        _services.AddLogging();
        return _services.AddKuvaNotificationsService(_configurationMock.Object, _environmentMock.Object);
    }

    [Test]
    public void AddKuvaNotificationsService_ReturnsSameServiceCollection()
    {
        // Act
        var result = BuildServices();

        // Assert
        Assert.That(result, Is.SameAs(_services));
    }

    [Test]
    public void AddKuvaNotificationsService_ConfiguresNotificationsOptions()
    {
        // Arrange
        var notificationsSection = new Mock<IConfigurationSection>();
        notificationsSection.Setup(s => s.Path).Returns("Notifications");
        notificationsSection.Setup(s => s.Key).Returns("Notifications");
        _configurationMock.Setup(c => c.GetSection("Notifications")).Returns(notificationsSection.Object);

        // Act
        BuildServices();

        // Assert
        _configurationMock.Verify(c => c.GetSection("Notifications"), Times.AtLeastOnce);
    }

    [Test]
    public void AddKuvaNotificationsService_ConfiguresSmtpOptions()
    {
        // Act
        BuildServices();

        // Assert
        _configurationMock.Verify(c => c.GetSection("Smtp"), Times.AtLeastOnce);
    }

    [Test]
    public void AddKuvaNotificationsService_ConfiguresSendGridOptions()
    {
        // Act
        BuildServices();

        // Assert
        _configurationMock.Verify(c => c.GetSection("SendGrid"), Times.AtLeastOnce);
    }

    [Test]
    public void AddKuvaNotificationsService_ConfiguresKeyVaultOptions()
    {
        // Act
        BuildServices();

        // Assert
        _configurationMock.Verify(c => c.GetSection("KeyVault"), Times.AtLeastOnce);
    }

    [Test]
    public void AddKuvaNotificationsService_RegistersIConfigureOptionsForNotificationOptions()
    {
        // Act
        BuildServices();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IConfigureOptions<NotificationOptions>)));
    }

    [Test]
    public void AddKuvaNotificationsService_RegistersIConfigureOptionsForSmtpOptions()
    {
        // Act
        BuildServices();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IConfigureOptions<SmtpOptions>)));
    }

    [Test]
    public void AddKuvaNotificationsService_RegistersIConfigureOptionsForSendGridOptions()
    {
        // Act
        BuildServices();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IConfigureOptions<SendGridOptions>)));
    }

    [Test]
    public void AddKuvaNotificationsService_RegistersIConfigureOptionsForKeyVaultOptions()
    {
        // Act
        BuildServices();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IConfigureOptions<KeyVaultOptions>)));
    }

    [Test]
    public void AddKuvaNotificationsService_RegistersAuthenticationServices()
    {
        // Act
        BuildServices();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IAuthenticationService)));
    }

    [Test]
    public void AddKuvaNotificationsService_RegistersJwtBearerOptions()
    {
        // Act
        BuildServices();

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(sd =>
            sd.ServiceType == typeof(IConfigureOptions<JwtBearerOptions>)));
    }

    [Test]
    public void AddKuvaNotificationsService_WhenDevelopment_SetsRequireHttpsMetadataToFalse()
    {
        // Arrange
        SetupJwtConfiguration("https://authority.example.com", "test-audience");

        // Act
        BuildServices(isDevelopment: true);
        var provider = _services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        // Assert
        Assert.That(options.RequireHttpsMetadata, Is.False);
    }

    [Test]
    public void AddKuvaNotificationsService_WhenProduction_SetsRequireHttpsMetadataToTrue()
    {
        // Arrange
        SetupJwtConfiguration("https://authority.example.com", "test-audience");

        // Act
        BuildServices(isDevelopment: false);
        var provider = _services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        // Assert
        Assert.That(options.RequireHttpsMetadata, Is.True);
    }

    [Test]
    public void AddKuvaNotificationsService_SetsJwtAuthorityFromConfiguration()
    {
        // Arrange
        const string authority = "https://my-auth-server.example.com";
        SetupJwtConfiguration(authority, "audience");

        // Act
        BuildServices();
        var provider = _services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        // Assert
        Assert.That(options.Authority, Is.EqualTo(authority));
    }

    [Test]
    public void AddKuvaNotificationsService_SetsJwtAudienceFromConfiguration()
    {
        // Arrange
        const string audience = "my-api-audience";
        SetupJwtConfiguration("https://authority.example.com", audience);

        // Act
        BuildServices();
        var provider = _services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        // Assert
        Assert.That(options.Audience, Is.EqualTo(audience));
    }

    [Test]
    public void AddKuvaNotificationsService_WhenJwtAuthorityIsNull_SetsAuthorityToNull()
    {
        // Arrange
        SetupJwtConfiguration(null!, "audience");

        // Act
        BuildServices();
        var provider = _services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        // Assert
        Assert.That(options.Authority, Is.Null);
    }
}
