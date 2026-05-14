using Kuva.Notifications.EFMigrations;
using Kuva.Notifications.Repository.Context;

namespace Kuva.Notifications.EFMigrations.UnitTests;

[TestFixture]
[NonParallelizable]
public sealed class DesignTimeDbContextFactoryTests
{
    private string _originalDirectory = string.Empty;
    private string _tempBaseDirectory = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _originalDirectory = Directory.GetCurrentDirectory();
        _tempBaseDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempBaseDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.SetCurrentDirectory(_originalDirectory);

        if (Directory.Exists(_tempBaseDirectory))
        {
            Directory.Delete(_tempBaseDirectory, recursive: true);
        }
    }

    private void SetUpAppsettingsJson(string connectionString)
    {
        // The factory uses Path.Combine(Directory.GetCurrentDirectory(), "../Kuva.Notifications.Service")
        // So we need: <tempBaseDirectory>/fake-app/../Kuva.Notifications.Service/appsettings.json
        var fakeAppDir = Path.Combine(_tempBaseDirectory, "fake-app");
        Directory.CreateDirectory(fakeAppDir);

        var serviceDir = Path.Combine(_tempBaseDirectory, "Kuva.Notifications.Service");
        Directory.CreateDirectory(serviceDir);

        var appsettings = $$"""
            {
              "ConnectionStrings": {
                "NotificationsDatabase": "{{connectionString}}"
              }
            }
            """;

        File.WriteAllText(Path.Combine(serviceDir, "appsettings.json"), appsettings);
        Directory.SetCurrentDirectory(fakeAppDir);
    }

    [Test]
    public void CreateDbContext_WhenAppsettingsJsonIsMissing_ThrowsException()
    {
        // Arrange
        var fakeDir = Path.Combine(_tempBaseDirectory, "fake-app-missing");
        var fakeServiceDir = Path.Combine(_tempBaseDirectory, "Kuva.Notifications.Service");
        Directory.CreateDirectory(fakeDir);
        Directory.CreateDirectory(fakeServiceDir); // directory exists, but appsettings.json does not
        Directory.SetCurrentDirectory(fakeDir);

        var factory = new DesignTimeDbContextFactory();

        // Act
        Action act = () => factory.CreateDbContext([]);

        // Assert
        Assert.That(() => act(), Throws.InstanceOf<FileNotFoundException>());
    }

    [Test]
    public void CreateDbContext_WhenServiceDirectoryIsMissing_ThrowsDirectoryNotFoundException()
    {
        // Arrange
        var fakeDir = Path.Combine(_tempBaseDirectory, "fake-app-missing");
        Directory.CreateDirectory(fakeDir);
        Directory.SetCurrentDirectory(fakeDir);

        var factory = new DesignTimeDbContextFactory();

        // Act
        Action act = () => factory.CreateDbContext([]);

        // Assert
        Assert.That(() => act(), Throws.InstanceOf<DirectoryNotFoundException>());
    }

    [Test]
    public void CreateDbContext_WhenConfigExists_ReturnsNotificationsDbContext()
    {
        // Arrange
        SetUpAppsettingsJson("Server=localhost;Database=TestDb;Trusted_Connection=True;");
        var factory = new DesignTimeDbContextFactory();

        // Act
        NotificationsDbContext result = factory.CreateDbContext([]);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NotificationsDbContext>());
    }

    [Test]
    public void CreateDbContext_WhenConfigExists_DisposesWithoutError()
    {
        // Arrange
        SetUpAppsettingsJson("Server=localhost;Database=TestDb;Trusted_Connection=True;");
        var factory = new DesignTimeDbContextFactory();

        // Act
        NotificationsDbContext result = factory.CreateDbContext([]);
        Action dispose = () => result.Dispose();

        // Assert
        Assert.DoesNotThrow(() => dispose());
    }

    [Test]
    public void CreateDbContext_WithEmptyArgs_ReturnsNotificationsDbContext()
    {
        // Arrange
        SetUpAppsettingsJson("Server=localhost;Database=NotificationsDb;Trusted_Connection=True;");
        var factory = new DesignTimeDbContextFactory();

        // Act
        NotificationsDbContext result = factory.CreateDbContext([]);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateDbContext_WhenDevelopmentAppsettingsPresent_ReturnsNotificationsDbContext()
    {
        // Arrange
        SetUpAppsettingsJson("Server=localhost;Database=TestDb;Trusted_Connection=True;");

        // Also write optional development appsettings
        var serviceDir = Path.Combine(_tempBaseDirectory, "Kuva.Notifications.Service");
        var devSettings = """
            {
              "ConnectionStrings": {
                "NotificationsDatabase": "Server=dev-server;Database=TestDb;Trusted_Connection=True;"
              }
            }
            """;
        File.WriteAllText(Path.Combine(serviceDir, "appsettings.Development.json"), devSettings);

        var factory = new DesignTimeDbContextFactory();

        // Act
        NotificationsDbContext result = factory.CreateDbContext([]);

        // Assert
        Assert.That(result, Is.Not.Null);
    }
}
