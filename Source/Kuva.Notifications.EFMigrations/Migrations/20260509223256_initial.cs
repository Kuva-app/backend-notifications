using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kuva.Notifications.EFMigrations.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationProviders", x => x.Id);
                });

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
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

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
                    table.ForeignKey(
                        name: "FK_NotificationRequests_NotificationTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                    table.ForeignKey(
                        name: "FK_NotificationAttachments_NotificationRequests_NotificationRequestId",
                        column: x => x.NotificationRequestId,
                        principalTable: "NotificationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_NotificationEvents_NotificationRequests_NotificationRequestId",
                        column: x => x.NotificationRequestId,
                        principalTable: "NotificationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_NotificationRequests_NotificationRequestId",
                        column: x => x.NotificationRequestId,
                        principalTable: "NotificationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_NotificationSendAttempts_NotificationProviders_NotificationProviderId",
                        column: x => x.NotificationProviderId,
                        principalTable: "NotificationProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NotificationSendAttempts_NotificationRequests_NotificationRequestId",
                        column: x => x.NotificationRequestId,
                        principalTable: "NotificationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "NotificationProviders",
                columns: new[] { "Id", "ConfigurationKey", "CreatedAtUtc", "IsActive", "Name", "Priority", "ProviderType", "Type", "UpdatedAtUtc" },
                values: new object[] { new Guid("7a40f1f4-5e62-4427-90c0-b89ac3cd0001"), "Fake", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Fake Local Provider", 1, 3, 1, null });

            migrationBuilder.InsertData(
                table: "NotificationTemplates",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description", "HtmlBodyTemplate", "IsActive", "Language", "Name", "RequiredVariablesJson", "SubjectTemplate", "TextBodyTemplate", "Type", "UpdatedAtUtc", "Version" },
                values: new object[,]
                {
                    { new Guid("7a40f1f4-5e62-4427-90c0-b89ac3cd0002"), "ORDER_RECEIVED", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "<h1>Ola, {{customerName}}</h1><p>Seu pedido {{orderNumber}} foi recebido pela loja {{storeName}}.</p>", true, "pt-BR", "Pedido recebido", "[\"customerName\",\"orderNumber\",\"storeName\"]", "Pedido {{orderNumber}} recebido pela loja", null, 1, null, 1 },
                    { new Guid("7a40f1f4-5e62-4427-90c0-b89ac3cd0003"), "ORDER_READY_FOR_PICKUP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "<h1>Ola, {{customerName}}</h1><p>Seu pedido esta pronto para retirada na loja {{storeName}}.</p>", true, "pt-BR", "Pedido pronto para retirada", "[\"customerName\",\"orderNumber\",\"storeName\"]", "Seu pedido {{orderNumber}} esta pronto para retirada", null, 1, null, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAttachments_NotificationRequestId",
                table: "NotificationAttachments",
                column: "NotificationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEvents_CreatedAtUtc",
                table: "NotificationEvents",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEvents_EventType",
                table: "NotificationEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEvents_NotificationRequestId",
                table: "NotificationEvents",
                column: "NotificationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationProviders_IsActive",
                table: "NotificationProviders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationProviders_Priority",
                table: "NotificationProviders",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationProviders_Type",
                table: "NotificationProviders",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_Address",
                table: "NotificationRecipients",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationRequestId",
                table: "NotificationRecipients",
                column: "NotificationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_CreatedAtUtc",
                table: "NotificationRequests",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_ExternalReference",
                table: "NotificationRequests",
                column: "ExternalReference");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_Status",
                table: "NotificationRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_TemplateCode",
                table: "NotificationRequests",
                column: "TemplateCode");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_TemplateId",
                table: "NotificationRequests",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_Type",
                table: "NotificationRequests",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_Type_TemplateId_ExternalReference",
                table: "NotificationRequests",
                columns: new[] { "Type", "TemplateId", "ExternalReference" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSendAttempts_NotificationProviderId",
                table: "NotificationSendAttempts",
                column: "NotificationProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSendAttempts_NotificationRequestId",
                table: "NotificationSendAttempts",
                column: "NotificationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSendAttempts_StartedAtUtc",
                table: "NotificationSendAttempts",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSendAttempts_Success",
                table: "NotificationSendAttempts",
                column: "Success");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Code",
                table: "NotificationTemplates",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Type",
                table: "NotificationTemplates",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Type_Code_IsActive",
                table: "NotificationTemplates",
                columns: new[] { "Type", "Code", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Type_Code_Version",
                table: "NotificationTemplates",
                columns: new[] { "Type", "Code", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationAttachments");

            migrationBuilder.DropTable(
                name: "NotificationEvents");

            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropTable(
                name: "NotificationSendAttempts");

            migrationBuilder.DropTable(
                name: "NotificationProviders");

            migrationBuilder.DropTable(
                name: "NotificationRequests");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");
        }
    }
}
