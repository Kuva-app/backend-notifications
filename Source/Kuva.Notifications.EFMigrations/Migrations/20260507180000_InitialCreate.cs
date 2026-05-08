using System;
using Kuva.Notifications.Repository.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kuva.Notifications.EFMigrations.Migrations;

[DbContext(typeof(NotificationsDbContext))]
[Migration("20260507180000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "NotificationProviders",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                ProviderType = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                Priority = table.Column<int>(type: "int", nullable: false),
                ConfigurationKey = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_NotificationProviders", x => x.Id));

        migrationBuilder.CreateTable(
            name: "NotificationTemplates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                SubjectTemplate = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                HtmlBodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TextBodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RequiredVariablesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_NotificationTemplates", x => x.Id));

        migrationBuilder.CreateTable(
            name: "NotificationRequests",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Type = table.Column<int>(type: "int", nullable: false),
                TemplateCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ExternalReference = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
                Priority = table.Column<int>(type: "int", nullable: false),
                VariablesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SubjectRendered = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationRequests", x => x.Id);
                table.ForeignKey("FK_NotificationRequests_NotificationTemplates_TemplateId", x => x.TemplateId, "NotificationTemplates", "Id", onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "NotificationAttachments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NotificationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                StorageReference = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationAttachments", x => x.Id);
                table.ForeignKey("FK_NotificationAttachments_NotificationRequests_NotificationRequestId", x => x.NotificationRequestId, "NotificationRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "NotificationEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NotificationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventType = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationEvents", x => x.Id);
                table.ForeignKey("FK_NotificationEvents_NotificationRequests_NotificationRequestId", x => x.NotificationRequestId, "NotificationRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "NotificationRecipients",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NotificationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Address = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationRecipients", x => x.Id);
                table.ForeignKey("FK_NotificationRecipients_NotificationRequests_NotificationRequestId", x => x.NotificationRequestId, "NotificationRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "NotificationSendAttempts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NotificationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NotificationProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                AttemptNumber = table.Column<int>(type: "int", nullable: false),
                Success = table.Column<bool>(type: "bit", nullable: false),
                ProviderMessageId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                FinishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationSendAttempts", x => x.Id);
                table.ForeignKey("FK_NotificationSendAttempts_NotificationProviders_NotificationProviderId", x => x.NotificationProviderId, "NotificationProviders", "Id", onDelete: ReferentialAction.SetNull);
                table.ForeignKey("FK_NotificationSendAttempts_NotificationRequests_NotificationRequestId", x => x.NotificationRequestId, "NotificationRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData("NotificationProviders", ["Id", "Name", "Type", "ProviderType", "IsActive", "Priority", "ConfigurationKey", "CreatedAtUtc"], [NotificationsDbContext.FakeProviderId, "Fake Local Provider", 1, 3, true, 1, "Fake", NotificationsDbContext.SeedCreatedAtUtc]);
        migrationBuilder.InsertData("NotificationTemplates", ["Id", "Type", "Code", "Name", "SubjectTemplate", "HtmlBodyTemplate", "RequiredVariablesJson", "Language", "Version", "IsActive", "CreatedAtUtc"], [NotificationsDbContext.OrderReceivedTemplateId, 1, "ORDER_RECEIVED", "Pedido recebido", "Pedido {{orderNumber}} recebido pela loja", "<h1>Ola, {{customerName}}</h1><p>Seu pedido {{orderNumber}} foi recebido pela loja {{storeName}}.</p>", "[\"customerName\",\"orderNumber\",\"storeName\"]", "pt-BR", 1, true, NotificationsDbContext.SeedCreatedAtUtc]);
        migrationBuilder.InsertData("NotificationTemplates", ["Id", "Type", "Code", "Name", "SubjectTemplate", "HtmlBodyTemplate", "RequiredVariablesJson", "Language", "Version", "IsActive", "CreatedAtUtc"], [NotificationsDbContext.OrderReadyTemplateId, 1, "ORDER_READY_FOR_PICKUP", "Pedido pronto para retirada", "Seu pedido {{orderNumber}} esta pronto para retirada", "<h1>Ola, {{customerName}}</h1><p>Seu pedido esta pronto para retirada na loja {{storeName}}.</p>", "[\"customerName\",\"orderNumber\",\"storeName\"]", "pt-BR", 1, true, NotificationsDbContext.SeedCreatedAtUtc]);

        migrationBuilder.CreateIndex("IX_NotificationAttachments_NotificationRequestId", "NotificationAttachments", "NotificationRequestId");
        migrationBuilder.CreateIndex("IX_NotificationEvents_CreatedAtUtc", "NotificationEvents", "CreatedAtUtc");
        migrationBuilder.CreateIndex("IX_NotificationEvents_NotificationRequestId", "NotificationEvents", "NotificationRequestId");
        migrationBuilder.CreateIndex("IX_NotificationEvents_EventType", "NotificationEvents", "EventType");
        migrationBuilder.CreateIndex("IX_NotificationProviders_Type", "NotificationProviders", "Type");
        migrationBuilder.CreateIndex("IX_NotificationProviders_IsActive", "NotificationProviders", "IsActive");
        migrationBuilder.CreateIndex("IX_NotificationProviders_Priority", "NotificationProviders", "Priority");
        migrationBuilder.CreateIndex("IX_NotificationRecipients_Address", "NotificationRecipients", "Address");
        migrationBuilder.CreateIndex("IX_NotificationRecipients_NotificationRequestId", "NotificationRecipients", "NotificationRequestId");
        migrationBuilder.CreateIndex("IX_NotificationRequests_CreatedAtUtc", "NotificationRequests", "CreatedAtUtc");
        migrationBuilder.CreateIndex("IX_NotificationRequests_ExternalReference", "NotificationRequests", "ExternalReference");
        migrationBuilder.CreateIndex("IX_NotificationRequests_Status", "NotificationRequests", "Status");
        migrationBuilder.CreateIndex("IX_NotificationRequests_TemplateCode", "NotificationRequests", "TemplateCode");
        migrationBuilder.CreateIndex("IX_NotificationRequests_Type", "NotificationRequests", "Type");
        migrationBuilder.CreateIndex("IX_NotificationRequests_Type_TemplateId_ExternalReference", "NotificationRequests", ["Type", "TemplateId", "ExternalReference"]);
        migrationBuilder.CreateIndex("IX_NotificationRequests_TemplateId", "NotificationRequests", "TemplateId");
        migrationBuilder.CreateIndex("IX_NotificationSendAttempts_NotificationProviderId", "NotificationSendAttempts", "NotificationProviderId");
        migrationBuilder.CreateIndex("IX_NotificationSendAttempts_NotificationRequestId", "NotificationSendAttempts", "NotificationRequestId");
        migrationBuilder.CreateIndex("IX_NotificationSendAttempts_StartedAtUtc", "NotificationSendAttempts", "StartedAtUtc");
        migrationBuilder.CreateIndex("IX_NotificationSendAttempts_Success", "NotificationSendAttempts", "Success");
        migrationBuilder.CreateIndex("IX_NotificationTemplates_Code", "NotificationTemplates", "Code");
        migrationBuilder.CreateIndex("IX_NotificationTemplates_Type", "NotificationTemplates", "Type");
        migrationBuilder.CreateIndex("IX_NotificationTemplates_Type_Code_IsActive", "NotificationTemplates", ["Type", "Code", "IsActive"]);
        migrationBuilder.CreateIndex("IX_NotificationTemplates_Type_Code_Version", "NotificationTemplates", ["Type", "Code", "Version"], unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("NotificationAttachments");
        migrationBuilder.DropTable("NotificationEvents");
        migrationBuilder.DropTable("NotificationRecipients");
        migrationBuilder.DropTable("NotificationSendAttempts");
        migrationBuilder.DropTable("NotificationProviders");
        migrationBuilder.DropTable("NotificationRequests");
        migrationBuilder.DropTable("NotificationTemplates");
    }
}
