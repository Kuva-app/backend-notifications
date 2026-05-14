using Kuva.Notifications.EFMigrations.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Kuva.Notifications.EFMigrations.UnitTests.Migrations;

[TestFixture]
public class InitialMigrationTests
{
    private TestableInitial _migration = null!;

    [SetUp]
    public void SetUp()
    {
        _migration = new TestableInitial();
    }

    [Test]
    public void Up_WhenCalled_CreatesNotificationProvidersTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<CreateTableOperation>().Count(op => op.Name == "NotificationProviders"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_CreatesNotificationTemplatesTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<CreateTableOperation>().Count(op => op.Name == "NotificationTemplates"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_CreatesNotificationRequestsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<CreateTableOperation>().Count(op => op.Name == "NotificationRequests"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_CreatesNotificationAttachmentsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<CreateTableOperation>().Count(op => op.Name == "NotificationAttachments"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_CreatesNotificationEventsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<CreateTableOperation>().Count(op => op.Name == "NotificationEvents"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_CreatesNotificationRecipientsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<CreateTableOperation>().Count(op => op.Name == "NotificationRecipients"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_CreatesNotificationSendAttemptsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<CreateTableOperation>().Count(op => op.Name == "NotificationSendAttempts"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_NotificationProvidersTableHasExpectedColumns()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationProviders");

        Assert.That(tableOp.Columns.Select(c => c.Name), Is.SupersetOf(new[]
        {
            "Id", "Name", "Type", "ProviderType", "IsActive", "Priority",
            "ConfigurationKey", "CreatedAtUtc", "UpdatedAtUtc"
        }));
    }

    [Test]
    public void Up_WhenCalled_NotificationTemplatesTableHasExpectedColumns()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationTemplates");

        Assert.That(tableOp.Columns.Select(c => c.Name), Is.SupersetOf(new[]
        {
            "Id", "Type", "Code", "Name", "Description", "SubjectTemplate",
            "HtmlBodyTemplate", "TextBodyTemplate", "RequiredVariablesJson",
            "Language", "Version", "IsActive", "CreatedAtUtc", "UpdatedAtUtc"
        }));
    }

    [Test]
    public void Up_WhenCalled_NotificationRequestsTableHasExpectedColumns()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationRequests");

        Assert.That(tableOp.Columns.Select(c => c.Name), Is.SupersetOf(new[]
        {
            "Id", "TemplateId", "Type", "TemplateCode", "ExternalReference",
            "Source", "Status", "Priority", "VariablesJson", "MetadataJson",
            "SubjectRendered", "ErrorMessage", "CreatedAtUtc", "UpdatedAtUtc", "SentAtUtc"
        }));
    }

    [Test]
    public void Up_WhenCalled_NotificationRequestsHasForeignKeyToTemplates()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationRequests");

        Assert.That(tableOp.ForeignKeys.Count(fk =>
            fk.Name == "FK_NotificationRequests_NotificationTemplates_TemplateId" &&
            fk.PrincipalTable == "NotificationTemplates"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_NotificationAttachmentsHasForeignKeyToRequests()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationAttachments");

        Assert.That(tableOp.ForeignKeys.Count(fk =>
            fk.Name == "FK_NotificationAttachments_NotificationRequests_NotificationRequestId" &&
            fk.PrincipalTable == "NotificationRequests"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_NotificationEventsHasForeignKeyToRequests()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationEvents");

        Assert.That(tableOp.ForeignKeys.Count(fk =>
            fk.Name == "FK_NotificationEvents_NotificationRequests_NotificationRequestId"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_NotificationRecipientsHasForeignKeyToRequests()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationRecipients");

        Assert.That(tableOp.ForeignKeys.Count(fk =>
            fk.Name == "FK_NotificationRecipients_NotificationRequests_NotificationRequestId"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_NotificationSendAttemptsHasTwoForeignKeys()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var tableOp = builder.Operations.OfType<CreateTableOperation>()
            .Single(t => t.Name == "NotificationSendAttempts");

        Assert.That(tableOp.ForeignKeys, Has.Count.EqualTo(2));
        Assert.That(tableOp.ForeignKeys.Select(fk => fk.Name), Is.SupersetOf(new[]
        {
            "FK_NotificationSendAttempts_NotificationProviders_NotificationProviderId",
            "FK_NotificationSendAttempts_NotificationRequests_NotificationRequestId"
        }));
    }

    [Test]
    public void Up_WhenCalled_InsertsNotificationProviderSeedData()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<InsertDataOperation>().Count(op => op.Table == "NotificationProviders"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_InsertsNotificationTemplateSeedData()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        Assert.That(builder.Operations.OfType<InsertDataOperation>().Count(op => op.Table == "NotificationTemplates"), Is.EqualTo(1));
    }

    [Test]
    public void Up_WhenCalled_InsertsTwoNotificationTemplates()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var insertOp = builder.Operations.OfType<InsertDataOperation>()
            .Single(o => o.Table == "NotificationTemplates");

        Assert.That(insertOp.Values.GetLength(0), Is.EqualTo(2));
    }

    [Test]
    public void Up_WhenCalled_CreatesExpectedIndexes()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var indexNames = builder.Operations.OfType<CreateIndexOperation>()
            .Select(i => i.Name)
            .ToList();

        Assert.That(indexNames, Is.SupersetOf(new[]
        {
            "IX_NotificationAttachments_NotificationRequestId",
            "IX_NotificationEvents_CreatedAtUtc",
            "IX_NotificationEvents_EventType",
            "IX_NotificationEvents_NotificationRequestId",
            "IX_NotificationProviders_IsActive",
            "IX_NotificationProviders_Priority",
            "IX_NotificationProviders_Type",
            "IX_NotificationRecipients_Address",
            "IX_NotificationRecipients_NotificationRequestId",
            "IX_NotificationRequests_CreatedAtUtc",
            "IX_NotificationRequests_ExternalReference",
            "IX_NotificationRequests_Status",
            "IX_NotificationRequests_TemplateCode",
            "IX_NotificationRequests_TemplateId",
            "IX_NotificationRequests_Type",
            "IX_NotificationRequests_Type_TemplateId_ExternalReference",
            "IX_NotificationSendAttempts_NotificationProviderId",
            "IX_NotificationSendAttempts_NotificationRequestId",
            "IX_NotificationSendAttempts_StartedAtUtc",
            "IX_NotificationSendAttempts_Success",
            "IX_NotificationTemplates_Code",
            "IX_NotificationTemplates_Type",
            "IX_NotificationTemplates_Type_Code_IsActive",
            "IX_NotificationTemplates_Type_Code_Version"
        }));
    }

    [Test]
    public void Up_WhenCalled_UniqueIndexOnTemplatesTypeCodeVersion()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallUp(builder);

        var uniqueIndex = builder.Operations.OfType<CreateIndexOperation>()
            .Single(i => i.Name == "IX_NotificationTemplates_Type_Code_Version");

        Assert.That(uniqueIndex.IsUnique, Is.True);
    }

    [Test]
    public void Down_WhenCalled_DropsAllSevenTables()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(), Is.EqualTo(7));
    }

    [Test]
    public void Down_WhenCalled_DropsNotificationAttachmentsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(op => op.Name == "NotificationAttachments"), Is.EqualTo(1));
    }

    [Test]
    public void Down_WhenCalled_DropsNotificationEventsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(op => op.Name == "NotificationEvents"), Is.EqualTo(1));
    }

    [Test]
    public void Down_WhenCalled_DropsNotificationRecipientsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(op => op.Name == "NotificationRecipients"), Is.EqualTo(1));
    }

    [Test]
    public void Down_WhenCalled_DropsNotificationSendAttemptsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(op => op.Name == "NotificationSendAttempts"), Is.EqualTo(1));
    }

    [Test]
    public void Down_WhenCalled_DropsNotificationProvidersTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(op => op.Name == "NotificationProviders"), Is.EqualTo(1));
    }

    [Test]
    public void Down_WhenCalled_DropsNotificationRequestsTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(op => op.Name == "NotificationRequests"), Is.EqualTo(1));
    }

    [Test]
    public void Down_WhenCalled_DropsNotificationTemplatesTable()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        Assert.That(builder.Operations.OfType<DropTableOperation>().Count(op => op.Name == "NotificationTemplates"), Is.EqualTo(1));
    }

    [Test]
    public void Down_WhenCalled_DropsTablesInCorrectOrder()
    {
        var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");

        _migration.CallDown(builder);

        var dropOps = builder.Operations.OfType<DropTableOperation>().Select(d => d.Name).ToList();

        Assert.That(dropOps.IndexOf("NotificationAttachments"), Is.LessThan(dropOps.IndexOf("NotificationRequests")));
        Assert.That(dropOps.IndexOf("NotificationEvents"), Is.LessThan(dropOps.IndexOf("NotificationRequests")));
        Assert.That(dropOps.IndexOf("NotificationRecipients"), Is.LessThan(dropOps.IndexOf("NotificationRequests")));
        Assert.That(dropOps.IndexOf("NotificationSendAttempts"), Is.LessThan(dropOps.IndexOf("NotificationRequests")));
        Assert.That(dropOps.IndexOf("NotificationRequests"), Is.LessThan(dropOps.IndexOf("NotificationTemplates")));
    }

    private sealed class TestableInitial : initial
    {
        public void CallUp(MigrationBuilder migrationBuilder) => Up(migrationBuilder);
        public void CallDown(MigrationBuilder migrationBuilder) => Down(migrationBuilder);
    }
}
