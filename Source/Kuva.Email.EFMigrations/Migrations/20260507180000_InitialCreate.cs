using System;
using Kuva.Email.Repository.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kuva.Email.EFMigrations.Migrations;

[DbContext(typeof(EmailDbContext))]
[Migration("20260507180000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EmailProviders",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ProviderType = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                Priority = table.Column<int>(type: "int", nullable: false),
                ConfigurationKey = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_EmailProviders", x => x.Id));

        migrationBuilder.CreateTable(
            name: "EmailTemplates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                SubjectTemplate = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                HtmlBodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TextBodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RequiredVariablesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_EmailTemplates", x => x.Id));

        migrationBuilder.CreateTable(
            name: "EmailRequests",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                table.PrimaryKey("PK_EmailRequests", x => x.Id);
                table.ForeignKey("FK_EmailRequests_EmailTemplates_TemplateId", x => x.TemplateId, "EmailTemplates", "Id", onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "EmailAttachments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmailRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                StorageReference = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailAttachments", x => x.Id);
                table.ForeignKey("FK_EmailAttachments_EmailRequests_EmailRequestId", x => x.EmailRequestId, "EmailRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EmailEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmailRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventType = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailEvents", x => x.Id);
                table.ForeignKey("FK_EmailEvents_EmailRequests_EmailRequestId", x => x.EmailRequestId, "EmailRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EmailRecipients",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmailRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailRecipients", x => x.Id);
                table.ForeignKey("FK_EmailRecipients_EmailRequests_EmailRequestId", x => x.EmailRequestId, "EmailRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EmailSendAttempts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmailRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmailProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                table.PrimaryKey("PK_EmailSendAttempts", x => x.Id);
                table.ForeignKey("FK_EmailSendAttempts_EmailProviders_EmailProviderId", x => x.EmailProviderId, "EmailProviders", "Id", onDelete: ReferentialAction.SetNull);
                table.ForeignKey("FK_EmailSendAttempts_EmailRequests_EmailRequestId", x => x.EmailRequestId, "EmailRequests", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData("EmailProviders", ["Id", "Name", "ProviderType", "IsActive", "Priority", "ConfigurationKey", "CreatedAtUtc"], [EmailDbContext.FakeProviderId, "Fake Local Provider", 3, true, 1, "Fake", EmailDbContext.SeedCreatedAtUtc]);
        migrationBuilder.InsertData("EmailTemplates", ["Id", "Code", "Name", "SubjectTemplate", "HtmlBodyTemplate", "RequiredVariablesJson", "Language", "Version", "IsActive", "CreatedAtUtc"], [EmailDbContext.OrderReceivedTemplateId, "ORDER_RECEIVED", "Pedido recebido", "Pedido {{orderNumber}} recebido pela loja", "<h1>Ola, {{customerName}}</h1><p>Seu pedido {{orderNumber}} foi recebido pela loja {{storeName}}.</p>", "[\"customerName\",\"orderNumber\",\"storeName\"]", "pt-BR", 1, true, EmailDbContext.SeedCreatedAtUtc]);
        migrationBuilder.InsertData("EmailTemplates", ["Id", "Code", "Name", "SubjectTemplate", "HtmlBodyTemplate", "RequiredVariablesJson", "Language", "Version", "IsActive", "CreatedAtUtc"], [EmailDbContext.OrderReadyTemplateId, "ORDER_READY_FOR_PICKUP", "Pedido pronto para retirada", "Seu pedido {{orderNumber}} esta pronto para retirada", "<h1>Ola, {{customerName}}</h1><p>Seu pedido esta pronto para retirada na loja {{storeName}}.</p>", "[\"customerName\",\"orderNumber\",\"storeName\"]", "pt-BR", 1, true, EmailDbContext.SeedCreatedAtUtc]);

        migrationBuilder.CreateIndex("IX_EmailAttachments_EmailRequestId", "EmailAttachments", "EmailRequestId");
        migrationBuilder.CreateIndex("IX_EmailEvents_CreatedAtUtc", "EmailEvents", "CreatedAtUtc");
        migrationBuilder.CreateIndex("IX_EmailEvents_EmailRequestId", "EmailEvents", "EmailRequestId");
        migrationBuilder.CreateIndex("IX_EmailEvents_EventType", "EmailEvents", "EventType");
        migrationBuilder.CreateIndex("IX_EmailProviders_IsActive", "EmailProviders", "IsActive");
        migrationBuilder.CreateIndex("IX_EmailProviders_Priority", "EmailProviders", "Priority");
        migrationBuilder.CreateIndex("IX_EmailRecipients_Email", "EmailRecipients", "Email");
        migrationBuilder.CreateIndex("IX_EmailRecipients_EmailRequestId", "EmailRecipients", "EmailRequestId");
        migrationBuilder.CreateIndex("IX_EmailRequests_CreatedAtUtc", "EmailRequests", "CreatedAtUtc");
        migrationBuilder.CreateIndex("IX_EmailRequests_ExternalReference", "EmailRequests", "ExternalReference");
        migrationBuilder.CreateIndex("IX_EmailRequests_Status", "EmailRequests", "Status");
        migrationBuilder.CreateIndex("IX_EmailRequests_TemplateCode", "EmailRequests", "TemplateCode");
        migrationBuilder.CreateIndex("IX_EmailRequests_TemplateCode_ExternalReference", "EmailRequests", ["TemplateCode", "ExternalReference"]);
        migrationBuilder.CreateIndex("IX_EmailRequests_TemplateId", "EmailRequests", "TemplateId");
        migrationBuilder.CreateIndex("IX_EmailSendAttempts_EmailProviderId", "EmailSendAttempts", "EmailProviderId");
        migrationBuilder.CreateIndex("IX_EmailSendAttempts_EmailRequestId", "EmailSendAttempts", "EmailRequestId");
        migrationBuilder.CreateIndex("IX_EmailSendAttempts_StartedAtUtc", "EmailSendAttempts", "StartedAtUtc");
        migrationBuilder.CreateIndex("IX_EmailSendAttempts_Success", "EmailSendAttempts", "Success");
        migrationBuilder.CreateIndex("IX_EmailTemplates_Code", "EmailTemplates", "Code");
        migrationBuilder.CreateIndex("IX_EmailTemplates_Code_IsActive", "EmailTemplates", ["Code", "IsActive"]);
        migrationBuilder.CreateIndex("IX_EmailTemplates_Code_Version", "EmailTemplates", ["Code", "Version"], unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("EmailAttachments");
        migrationBuilder.DropTable("EmailEvents");
        migrationBuilder.DropTable("EmailRecipients");
        migrationBuilder.DropTable("EmailSendAttempts");
        migrationBuilder.DropTable("EmailProviders");
        migrationBuilder.DropTable("EmailRequests");
        migrationBuilder.DropTable("EmailTemplates");
    }
}
