using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Repository.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kuva.Notifications.Tests.Configurations;

[TestFixture]
public sealed class NotificationTemplateConfigurationTests
{
    private static IReadOnlyEntityType GetEntityType()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new NotificationTemplateConfiguration());
        var model = modelBuilder.FinalizeModel();
        return model.FindEntityType(typeof(NotificationTemplate))!;
    }

    [Test]
    public void Configure_TableName_ShouldBeNotificationTemplates()
    {
        var entityType = GetEntityType();

        Assert.That(entityType.GetTableName(), Is.EqualTo("NotificationTemplates"));
    }

    [Test]
    public void Configure_PrimaryKey_ShouldBeId()
    {
        var entityType = GetEntityType();

        var key = entityType.FindPrimaryKey();
        Assert.That(key, Is.Not.Null);
        Assert.That(key!.Properties.Count(p => p.Name == nameof(NotificationTemplate.Id)), Is.EqualTo(1));
    }

    [Test]
    public void Configure_TypeProperty_ShouldHaveIntConversion()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Type))!;
        Assert.That(property.GetProviderClrType(), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void Configure_TypeProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Type))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_CodeProperty_ShouldHaveMaxLength100()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Code))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(100));
    }

    [Test]
    public void Configure_CodeProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Code))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_NameProperty_ShouldHaveMaxLength150()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Name))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(150));
    }

    [Test]
    public void Configure_NameProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Name))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_DescriptionProperty_ShouldHaveMaxLength500()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Description))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(500));
    }

    [Test]
    public void Configure_DescriptionProperty_ShouldBeNullable()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Description))!;
        Assert.That(property.IsNullable, Is.True);
    }

    [Test]
    public void Configure_SubjectTemplateProperty_ShouldHaveMaxLength300()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.SubjectTemplate))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(300));
    }

    [Test]
    public void Configure_SubjectTemplateProperty_ShouldBeNullable()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.SubjectTemplate))!;
        Assert.That(property.IsNullable, Is.True);
    }

    [Test]
    public void Configure_HtmlBodyTemplateProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.HtmlBodyTemplate))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_RequiredVariablesJsonProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.RequiredVariablesJson))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_LanguageProperty_ShouldHaveMaxLength10()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Language))!;
        Assert.That(property.GetMaxLength(), Is.EqualTo(10));
    }

    [Test]
    public void Configure_LanguageProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.Language))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_CreatedAtUtcProperty_ShouldBeRequired()
    {
        var entityType = GetEntityType();

        var property = entityType.FindProperty(nameof(NotificationTemplate.CreatedAtUtc))!;
        Assert.That(property.IsNullable, Is.False);
    }

    [Test]
    public void Configure_TypeIndex_ShouldExistWithCorrectDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes().FirstOrDefault(i =>
            i.Properties.Count == 1 && i.Properties[0].Name == nameof(NotificationTemplate.Type));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationTemplates_Type"));
    }

    [Test]
    public void Configure_CodeIndex_ShouldExistWithCorrectDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes().FirstOrDefault(i =>
            i.Properties.Count == 1 && i.Properties[0].Name == nameof(NotificationTemplate.Code));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationTemplates_Code"));
    }

    [Test]
    public void Configure_TypeCodeIsActiveIndex_ShouldExistWithCorrectDatabaseName()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes().FirstOrDefault(i =>
            i.Properties.Count == 3 &&
            i.Properties.Any(p => p.Name == nameof(NotificationTemplate.Type)) &&
            i.Properties.Any(p => p.Name == nameof(NotificationTemplate.Code)) &&
            i.Properties.Any(p => p.Name == nameof(NotificationTemplate.IsActive)));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.GetDatabaseName(), Is.EqualTo("IX_NotificationTemplates_Type_Code_IsActive"));
    }

    [Test]
    public void Configure_TypeCodeVersionIndex_ShouldBeUnique()
    {
        var entityType = GetEntityType();

        var index = entityType.GetIndexes().FirstOrDefault(i =>
            i.Properties.Count == 3 &&
            i.Properties.Any(p => p.Name == nameof(NotificationTemplate.Type)) &&
            i.Properties.Any(p => p.Name == nameof(NotificationTemplate.Code)) &&
            i.Properties.Any(p => p.Name == nameof(NotificationTemplate.Version)));
        Assert.That(index, Is.Not.Null);
        Assert.That(index!.IsUnique, Is.True);
    }
}
