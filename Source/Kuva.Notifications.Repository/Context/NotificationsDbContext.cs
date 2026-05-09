using Kuva.Notifications.Entities.Entities;
using Kuva.Notifications.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Notifications.Repository.Context;

public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public static readonly Guid FakeProviderId = Guid.Parse("7a40f1f4-5e62-4427-90c0-b89ac3cd0001");
    public static readonly Guid OrderReceivedTemplateId = Guid.Parse("7a40f1f4-5e62-4427-90c0-b89ac3cd0002");
    public static readonly Guid OrderReadyTemplateId = Guid.Parse("7a40f1f4-5e62-4427-90c0-b89ac3cd0003");
    public static readonly DateTime SeedCreatedAtUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationRequest> NotificationRequests => Set<NotificationRequest>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<NotificationAttachment> NotificationAttachments => Set<NotificationAttachment>();
    public DbSet<NotificationProvider> NotificationProviders => Set<NotificationProvider>();
    public DbSet<NotificationSendAttempt> NotificationSendAttempts => Set<NotificationSendAttempt>();
    public DbSet<NotificationEvent> NotificationEvents => Set<NotificationEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
        Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationProvider>().HasData(new NotificationProvider
        {
            Id = FakeProviderId,
            Name = "Fake Local Provider",
            Type = NotificationType.Email,
            ProviderType = NotificationProviderType.Fake,
            IsActive = true,
            Priority = 1,
            ConfigurationKey = "Fake",
            CreatedAtUtc = SeedCreatedAtUtc
        });

        modelBuilder.Entity<NotificationTemplate>().HasData(
            new NotificationTemplate
            {
                Id = OrderReceivedTemplateId,
                Type = NotificationType.Email,
                Code = "ORDER_RECEIVED",
                Name = "Pedido recebido",
                SubjectTemplate = "Pedido {{orderNumber}} recebido pela loja",
                HtmlBodyTemplate = "<h1>Ola, {{customerName}}</h1><p>Seu pedido {{orderNumber}} foi recebido pela loja {{storeName}}.</p>",
                RequiredVariablesJson = """["customerName","orderNumber","storeName"]""",
                Language = "pt-BR",
                Version = 1,
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new NotificationTemplate
            {
                Id = OrderReadyTemplateId,
                Type = NotificationType.Email,
                Code = "ORDER_READY_FOR_PICKUP",
                Name = "Pedido pronto para retirada",
                SubjectTemplate = "Seu pedido {{orderNumber}} esta pronto para retirada",
                HtmlBodyTemplate = "<h1>Ola, {{customerName}}</h1><p>Seu pedido esta pronto para retirada na loja {{storeName}}.</p>",
                RequiredVariablesJson = """["customerName","orderNumber","storeName"]""",
                Language = "pt-BR",
                Version = 1,
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            });
    }
}
