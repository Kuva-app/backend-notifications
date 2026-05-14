using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Repository.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NUnit.Framework;

namespace Kuva.Notifications.Tests.Configurations;

[TestFixture]
public sealed class NotificationSendAttemptConfigurationTests
{
    private IEntityType _entityType = null!;

    [SetUp]
    public void SetUp()
    {
        var modelBuilder = new ModelBuilder();
        var configuration = new NotificationSendAttemptConfiguration();
        configuration.Configure(modelBuilder.Entity<NotificationSendAttempt>());

        var model = modelBuilder.FinalizeModel();
        _entityType = model.FindEntityType(typeof(NotificationSendAttempt))!;
    }

    [Test]
    public void Configure_TableName_ShouldBeNotificationSendAttempts()
    {
        Assert.That(_entityType.GetTableName(), Is.EqualTo("NotificationSendAttempts"));
    }

    [Test]
    public void Configure_PrimaryKey_ShouldBeId()
    {
        var key = _entityType.FindPrimaryKey();
        Assert.That(key, Is.Not.Null);
        Assert.That(key!.Properties.Count(p => p.Name == "Id"), Is.EqualTo(1));
    }

    [Test]
    public void Configure_ProviderMessageId_MaxLengthShouldBe200()
    {
        var property = _entityType.FindProperty(nameof(NotificationSendAttempt.ProviderMessageId));
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.GetMaxLength(), Is.EqualTo(200));
    }

    [Test]
    public void Configure_ErrorCode_MaxLengthShouldBe100()
    {
        var property = _entityType.FindProperty(nameof(NotificationSendAttempt.ErrorCode));
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.GetMaxLength(), Is.EqualTo(100));
    }

    [Test]
    public void Configure_ErrorMessage_MaxLengthShouldBe1000()
    {
        var property = _entityType.FindProperty(nameof(NotificationSendAttempt.ErrorMessage));
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.GetMaxLength(), Is.EqualTo(1000));
    }

    [Test]
    public void Configure_StartedAtUtc_ShouldBeRequired()
    {
        var property = _entityType.FindProperty(nameof(NotificationSendAttempt.StartedAtUtc));
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.IsNullable, Is.False);
    }

    [Test]
    public void Configure_NotificationProviderRelationship_ShouldHaveForeignKeyNotificationProviderId()
    {
        var fk = _entityType.GetForeignKeys()
            .FirstOrDefault(f => f.Properties.Any(p => p.Name == "NotificationProviderId"));
        Assert.That(fk, Is.Not.Null);
    }

    [Test]
    public void Configure_NotificationProviderRelationship_DeleteBehaviorShouldBeSetNull()
    {
        var fk = _entityType.GetForeignKeys()
            .First(f => f.Properties.Any(p => p.Name == "NotificationProviderId"));
        Assert.That(fk.DeleteBehavior, Is.EqualTo(DeleteBehavior.SetNull));
    }

    [Test]
    public void Configure_IndexOnNotificationRequestId_ShouldExistWithCorrectDatabaseName()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == "NotificationRequestId"));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationSendAttempts_NotificationRequestId"));
    }

    [Test]
    public void Configure_IndexOnSuccess_ShouldExistWithCorrectDatabaseName()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == "Success"));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationSendAttempts_Success"));
    }

    [Test]
    public void Configure_IndexOnStartedAtUtc_ShouldExistWithCorrectDatabaseName()
    {
        var index = _entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == "StartedAtUtc"));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationSendAttempts_StartedAtUtc"));
    }

    [Test]
    public void Configure_NotificationProviderNavigation_ShouldBeToMany()
    {
        var fk = _entityType.GetForeignKeys()
            .First(f => f.Properties.Any(p => p.Name == "NotificationProviderId"));
        Assert.That(fk.IsUnique, Is.False);
    }
}
