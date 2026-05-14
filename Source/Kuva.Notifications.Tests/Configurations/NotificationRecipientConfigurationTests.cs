using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Repository.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kuva.Notifications.Tests.Configurations;

[TestFixture]
public sealed class NotificationRecipientConfigurationTests
{
    private static IReadOnlyEntityType GetEntityType()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new NotificationRecipientConfiguration());
        var model = modelBuilder.FinalizeModel();
        return model.FindEntityType(typeof(NotificationRecipient))!;
    }

    [Test]
    public void Configure_TableName_ShouldBeNotificationRecipients()
    {
        var entityType = GetEntityType();

        Assert.That(entityType.GetTableName(), Is.EqualTo("NotificationRecipients"));
    }

    [Test]
    public void Configure_PrimaryKey_ShouldBeId()
    {
        var entityType = GetEntityType();

        var key = entityType.FindPrimaryKey();
        Assert.That(key, Is.Not.Null);
        Assert.That(key!.Properties.Count(p => p.Name == nameof(NotificationRecipient.Id)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_AddressProperty_ShouldHaveMaxLength320()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRecipient.Address))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(320));
    }

    [Test]
    public void Configure_AddressProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRecipient.Address))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_NameProperty_ShouldHaveMaxLength150()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRecipient.Name))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(150));
    }

    [Test]
    public void Configure_RoleProperty_ShouldHaveMaxLength30()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRecipient.Role))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(30));
    }

    [Test]
    public void Configure_RoleProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRecipient.Role))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_CreatedAtUtcProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRecipient.CreatedAtUtc))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_NotificationRequestIdIndex_ShouldExistWithExpectedDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationRecipient.NotificationRequestId)));

        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationRecipients_NotificationRequestId"));
    }

    [Test]
    public void Configure_AddressIndex_ShouldExistWithExpectedDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationRecipient.Address)));

        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationRecipients_Address"));
    }
}
