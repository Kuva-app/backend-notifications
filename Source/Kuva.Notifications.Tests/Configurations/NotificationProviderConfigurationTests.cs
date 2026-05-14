using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Repository.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kuva.Notifications.Tests.Configurations;

[TestFixture]
public sealed class NotificationProviderConfigurationTests
{
    private static IReadOnlyEntityType GetEntityType()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new NotificationProviderConfiguration());
        var model = modelBuilder.FinalizeModel();
        return model.FindEntityType(typeof(NotificationProvider))!;
    }

    [Test]
    public void Configure_TableName_ShouldBeNotificationProviders()
    {
        var entityType = GetEntityType();

        Assert.That(entityType.GetTableName(), Is.EqualTo("NotificationProviders"));
    }

    [Test]
    public void Configure_PrimaryKey_ShouldBeId()
    {
        var entityType = GetEntityType();

        var key = entityType.FindPrimaryKey();
        Assert.That(key, Is.Not.Null);
        Assert.That(key!.Properties.Count(p => p.Name == nameof(NotificationProvider.Id)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_NameProperty_ShouldHaveMaxLength100()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.Name))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(100));
    }

    [Test]
    public void Configure_NameProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.Name))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_TypeProperty_ShouldHaveIntConversion()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.Type))!;
        Assert.That(property.GetProviderClrType(), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void Configure_TypeProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.Type))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_ProviderTypeProperty_ShouldHaveIntConversion()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.ProviderType))!;
        Assert.That(property.GetProviderClrType(), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void Configure_ProviderTypeProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.ProviderType))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_ConfigurationKeyProperty_ShouldHaveMaxLength150()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.ConfigurationKey))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(150));
    }

    [Test]
    public void Configure_CreatedAtUtcProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationProvider.CreatedAtUtc))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_TypeIndex_ShouldExistWithExpectedDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationProvider.Type)));

        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationProviders_Type"));
    }

    [Test]
    public void Configure_IsActiveIndex_ShouldExistWithExpectedDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationProvider.IsActive)));

        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationProviders_IsActive"));
    }

    [Test]
    public void Configure_PriorityIndex_ShouldExistWithExpectedDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationProvider.Priority)));

        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationProviders_Priority"));
    }
}
