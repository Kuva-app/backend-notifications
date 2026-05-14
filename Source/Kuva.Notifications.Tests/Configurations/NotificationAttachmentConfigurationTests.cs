using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Repository.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kuva.Notifications.Tests.Configurations;

[TestFixture]
public sealed class NotificationAttachmentConfigurationTests
{
    private static IReadOnlyEntityType GetEntityType()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new NotificationAttachmentConfiguration());
        var model = modelBuilder.FinalizeModel();
        return model.FindEntityType(typeof(NotificationAttachment))!;
    }

    [Test]
    public void Configure_TableName_ShouldBeNotificationAttachments()
    {
        var entityType = GetEntityType();

        Assert.That(entityType.GetTableName(), Is.EqualTo("NotificationAttachments"));
    }

    [Test]
    public void Configure_PrimaryKey_ShouldBeId()
    {
        var entityType = GetEntityType();

        var key = entityType.FindPrimaryKey();
        Assert.That(key, Is.Not.Null);
        Assert.That(key!.Properties.Count(p => p.Name == nameof(NotificationAttachment.Id)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_FileNameProperty_ShouldHaveMaxLength255()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationAttachment.FileName))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(255));
    }

    [Test]
    public void Configure_FileNameProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationAttachment.FileName))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_ContentTypeProperty_ShouldHaveMaxLength100()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationAttachment.ContentType))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(100));
    }

    [Test]
    public void Configure_ContentTypeProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationAttachment.ContentType))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_StorageReferenceProperty_ShouldHaveMaxLength1000()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationAttachment.StorageReference))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(1000));
    }

    [Test]
    public void Configure_StorageReferenceProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationAttachment.StorageReference))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_CreatedAtUtcProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationAttachment.CreatedAtUtc))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_NotificationRequestIdIndex_ShouldExistWithExpectedDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(NotificationAttachment.NotificationRequestId)));

        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationAttachments_NotificationRequestId"));
    }
}
