using Kuva.Notifications.Service.Extensions;
using Microsoft.Extensions.Configuration;

namespace Kuva.Notifications.Tests.Extensions;

[TestFixture]
public sealed class KeyVaultExtensionsTests
{
    private static IConfigurationBuilder CreateBuilder(string? keyVaultUri)
    {
        var builder = new ConfigurationBuilder();

        if (keyVaultUri is not null)
        {
            builder.AddInMemoryCollection(new Dictionary<string, string?> { ["KeyVault:Uri"] = keyVaultUri });
        }

        return builder;
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsNull_ReturnsSameBuilder()
    {
        // Arrange
        var builder = CreateBuilder(null);

        // Act
        var result = builder.AddKeyVaultIfConfigured();

        // Assert
        Assert.That(result, Is.SameAs(builder));
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsEmpty_ReturnsSameBuilder()
    {
        // Arrange
        var builder = CreateBuilder(string.Empty);

        // Act
        var result = builder.AddKeyVaultIfConfigured();

        // Assert
        Assert.That(result, Is.SameAs(builder));
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsWhitespace_ReturnsSameBuilder()
    {
        // Arrange
        var builder = CreateBuilder("   ");

        // Act
        var result = builder.AddKeyVaultIfConfigured();

        // Assert
        Assert.That(result, Is.SameAs(builder));
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsNull_DoesNotAddKeyVaultSource()
    {
        // Arrange
        var builder = CreateBuilder(null);
        var sourcesCountBefore = builder.Sources.Count;

        // Act
        builder.AddKeyVaultIfConfigured();

        // Assert
        // Build() internally adds a source; net change should be zero (no AzureKeyVault source added)
        Assert.That(builder.Sources.Count, Is.EqualTo(sourcesCountBefore));
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsEmpty_DoesNotAddKeyVaultSource()
    {
        // Arrange
        var builder = CreateBuilder(string.Empty);
        var sourcesCountBefore = builder.Sources.Count;

        // Act
        builder.AddKeyVaultIfConfigured();

        // Assert
        Assert.That(builder.Sources.Count, Is.EqualTo(sourcesCountBefore));
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsWhitespace_DoesNotAddKeyVaultSource()
    {
        // Arrange
        var builder = CreateBuilder("   ");
        var sourcesCountBefore = builder.Sources.Count;

        // Act
        builder.AddKeyVaultIfConfigured();

        // Assert
        Assert.That(builder.Sources.Count, Is.EqualTo(sourcesCountBefore));
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsConfigured_ReturnsSameBuilder()
    {
        // Arrange
        var builder = CreateBuilder("https://myvault.vault.azure.net/");

        // Act
        var result = builder.AddKeyVaultIfConfigured();

        // Assert
        Assert.That(result, Is.SameAs(builder));
    }

    [Test]
    public void AddKeyVaultIfConfigured_WhenKeyVaultUriIsConfigured_AddsKeyVaultSource()
    {
        // Arrange
        var builder = CreateBuilder("https://myvault.vault.azure.net/");
        var sourcesCountBefore = builder.Sources.Count;

        // Act
        builder.AddKeyVaultIfConfigured();

        // Assert
        Assert.That(builder.Sources.Count, Is.GreaterThan(sourcesCountBefore));
    }
}
