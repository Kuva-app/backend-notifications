using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Repository.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kuva.Notifications.Tests.Configurations;

[TestFixture]
public sealed class NotificationRequestConfigurationTests
{
    private static IReadOnlyEntityType GetEntityType()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new NotificationRequestConfiguration());
        var model = modelBuilder.FinalizeModel();
        return model.FindEntityType(typeof(NotificationRequest))!;
    }

    [Test]
    public void Configure_TableName_ShouldBeNotificationRequests()
    {
        var entityType = GetEntityType();

        Assert.That(entityType.GetTableName(), Is.EqualTo("NotificationRequests"));
    }

    [Test]
    public void Configure_PrimaryKey_ShouldBeId()
    {
        var entityType = GetEntityType();

        var key = entityType.FindPrimaryKey();
        Assert.That(key, Is.Not.Null);
        Assert.That(key!.Properties.Count(p => p.Name == nameof(NotificationRequest.Id)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_TypeProperty_ShouldHaveIntConversion()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.Type))!;
        Assert.That(property.GetProviderClrType(), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void Configure_TypeProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.Type))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_TemplateCodeProperty_ShouldHaveMaxLength100()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.TemplateCode))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(100));
    }

    [Test]
    public void Configure_TemplateCodeProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.TemplateCode))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_ExternalReferenceProperty_ShouldHaveMaxLength150()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.ExternalReference))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(150));
    }

    [Test]
    public void Configure_SourceProperty_ShouldHaveMaxLength100()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.Source))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(100));
    }

    [Test]
    public void Configure_StatusProperty_ShouldHaveIntConversion()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.Status))!;
        Assert.That(property.GetProviderClrType(), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void Configure_StatusProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.Status))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_PriorityProperty_ShouldHaveIntConversion()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.Priority))!;
        Assert.That(property.GetProviderClrType(), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void Configure_PriorityProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.Priority))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_VariablesJsonProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.VariablesJson))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_SubjectRenderedProperty_ShouldHaveMaxLength300()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.SubjectRendered))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(300));
    }

    [Test]
    public void Configure_ErrorMessageProperty_ShouldHaveMaxLength1000()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.ErrorMessage))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(1000));
    }

    [Test]
    public void Configure_CreatedAtUtcProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationRequest.CreatedAtUtc))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_TemplateRelationship_ShouldHaveSetNullDeleteBehavior()
    {
        var entityType = GetEntityType();

        var fk = entityType.GetForeignKeys()
            .SingleOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(NotificationTemplate));
        Assert.That(fk, Is.Not.Null);
        Assert.That(fk!.DeleteBehavior, Is.EqualTo(DeleteBehavior.SetNull));
    }

    [Test]
    public void Configure_RecipientsRelationship_ShouldHaveCascadeDeleteBehavior()
    {
        var entityType = GetEntityType();

        var fk = entityType.GetReferencingForeignKeys()
            .SingleOrDefault(fk => fk.DeclaringEntityType.ClrType == typeof(NotificationRecipient));
        Assert.That(fk, Is.Not.Null);
        Assert.That(fk!.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
    }

    [Test]
    public void Configure_AttachmentsRelationship_ShouldHaveCascadeDeleteBehavior()
    {
        var entityType = GetEntityType();

        var fk = entityType.GetReferencingForeignKeys()
            .SingleOrDefault(fk => fk.DeclaringEntityType.ClrType == typeof(NotificationAttachment));
        Assert.That(fk, Is.Not.Null);
        Assert.That(fk!.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
    }

    [Test]
    public void Configure_AttemptsRelationship_ShouldHaveCascadeDeleteBehavior()
    {
        var entityType = GetEntityType();

        var fk = entityType.GetReferencingForeignKeys()
            .SingleOrDefault(fk => fk.DeclaringEntityType.ClrType == typeof(NotificationSendAttempt));
        Assert.That(fk, Is.Not.Null);
        Assert.That(fk!.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
    }

    [Test]
    public void Configure_EventsRelationship_ShouldHaveCascadeDeleteBehavior()
    {
        var entityType = GetEntityType();

        var fk = entityType.GetReferencingForeignKeys()
            .SingleOrDefault(fk => fk.DeclaringEntityType.ClrType == typeof(NotificationEvent));
        Assert.That(fk, Is.Not.Null);
        Assert.That(fk!.DeleteBehavior, Is.EqualTo(DeleteBehavior.Cascade));
    }

    [Test]
    public void Configure_TypeIndex_ShouldExistWithCorrectName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.GetDatabaseName() == "IX_NotificationRequests_Type");
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.Properties.Count(p => p.Name == nameof(NotificationRequest.Type)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_TemplateCodeIndex_ShouldExistWithCorrectName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.GetDatabaseName() == "IX_NotificationRequests_TemplateCode");
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.Properties.Count(p => p.Name == nameof(NotificationRequest.TemplateCode)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_StatusIndex_ShouldExistWithCorrectName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.GetDatabaseName() == "IX_NotificationRequests_Status");
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.Properties.Count(p => p.Name == nameof(NotificationRequest.Status)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_ExternalReferenceIndex_ShouldExistWithCorrectName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.GetDatabaseName() == "IX_NotificationRequests_ExternalReference");
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.Properties.Count(p => p.Name == nameof(NotificationRequest.ExternalReference)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_CreatedAtUtcIndex_ShouldExistWithCorrectName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.GetDatabaseName() == "IX_NotificationRequests_CreatedAtUtc");
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.Properties.Count(p => p.Name == nameof(NotificationRequest.CreatedAtUtc)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_CompositeIndex_ShouldExistWithCorrectName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.GetDatabaseName() == "IX_NotificationRequests_Type_TemplateId_ExternalReference");
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.Properties.Select(p => p.Name), Is.EquivalentTo(
            [nameof(NotificationRequest.Type), nameof(NotificationRequest.TemplateId), nameof(NotificationRequest.ExternalReference)]));
    }
}
