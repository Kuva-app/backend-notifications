using Kuva.Notifications.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kuva.Notifications.Tests.Configurations;

[TestFixture]
public sealed class NotificationEventConfigurationTests : TestBase
{
    private IEntityType _entityType = null!;

    [SetUp]
    public void SetUp()
    {
        using var dbContext = CreateDbContext();
        _entityType = dbContext.Model.FindEntityType(typeof(NotificationEvent))!;
    }

    [Test]
    public void Configure_TableName_ShouldBeNotificationEvents()
    {
        Assert.That(_entityType.GetTableName(), Is.EqualTo("NotificationEvents"));
    }

    [Test]
    public void Configure_PrimaryKey_ShouldBeId()
    {
        var key = _entityType.FindPrimaryKey();
        Assert.That(key, Is.Not.Null);
        Assert.That(key!.Properties.Count(p => p.Name == "Id"), Is.EqualTo(1));
    }

    [Test]
    public void Configure_EventTypeProperty_ShouldBeRequired()
    {
        var property = _entityType.FindProperty(nameof(NotificationEvent.EventType));
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.IsNullable, Is.False);
    }

    [Test]
    public void Configure_EventTypeProperty_ShouldHaveIntConversion()
    {
        var property = _entityType.FindProperty(nameof(NotificationEvent.EventType));
        Assert.That(property, Is.Not.Null);
        var providerType = property!.GetProviderClrType();
        Assert.That(providerType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void Configure_DescriptionProperty_ShouldHaveMaxLength500()
    {
        var property = _entityType.FindProperty(nameof(NotificationEvent.Description));
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.GetMaxLength(), Is.EqualTo(500));
    }

    [Test]
    public void Configure_CreatedAtUtcProperty_ShouldBeRequired()
    {
        var property = _entityType.FindProperty(nameof(NotificationEvent.CreatedAtUtc));
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.IsNullable, Is.False);
    }

    [Test]
    public void Configure_NotificationRequestIdIndex_ShouldExistWithCorrectDatabaseName()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationEvent.NotificationRequestId)));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationEvents_NotificationRequestId"));
    }

    [Test]
    public void Configure_EventTypeIndex_ShouldExistWithCorrectDatabaseName()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationEvent.EventType)));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationEvents_EventType"));
    }

    [Test]
    public void Configure_CreatedAtUtcIndex_ShouldExistWithCorrectDatabaseName()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationEvent.CreatedAtUtc)));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationEvents_CreatedAtUtc"));
    }
}
