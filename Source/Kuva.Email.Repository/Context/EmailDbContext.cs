using Kuva.Email.Entities.Entities;
using Kuva.Email.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kuva.Email.Repository.Context;

public sealed class EmailDbContext(DbContextOptions<EmailDbContext> options) : DbContext(options)
{
    public static readonly Guid FakeProviderId = Guid.Parse("7a40f1f4-5e62-4427-90c0-b89ac3cd0001");
    public static readonly Guid OrderReceivedTemplateId = Guid.Parse("7a40f1f4-5e62-4427-90c0-b89ac3cd0002");
    public static readonly Guid OrderReadyTemplateId = Guid.Parse("7a40f1f4-5e62-4427-90c0-b89ac3cd0003");
    public static readonly DateTime SeedCreatedAtUtc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<EmailRequest> EmailRequests => Set<EmailRequest>();
    public DbSet<EmailRecipient> EmailRecipients => Set<EmailRecipient>();
    public DbSet<EmailAttachment> EmailAttachments => Set<EmailAttachment>();
    public DbSet<EmailProvider> EmailProviders => Set<EmailProvider>();
    public DbSet<EmailSendAttempt> EmailSendAttempts => Set<EmailSendAttempt>();
    public DbSet<EmailEvent> EmailEvents => Set<EmailEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmailDbContext).Assembly);
        Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailProvider>().HasData(new EmailProvider
        {
            Id = FakeProviderId,
            Name = "Fake Local Provider",
            ProviderType = EmailProviderType.Fake,
            IsActive = true,
            Priority = 1,
            ConfigurationKey = "Fake",
            CreatedAtUtc = SeedCreatedAtUtc
        });

        modelBuilder.Entity<EmailTemplate>().HasData(
            new EmailTemplate
            {
                Id = OrderReceivedTemplateId,
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
            new EmailTemplate
            {
                Id = OrderReadyTemplateId,
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
