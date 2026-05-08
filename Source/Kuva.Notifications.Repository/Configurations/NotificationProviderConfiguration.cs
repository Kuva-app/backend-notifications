using Kuva.Notifications.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Notifications.Repository.Configurations;

public sealed class NotificationProviderConfiguration : IEntityTypeConfiguration<NotificationProvider>
{
    public void Configure(EntityTypeBuilder<NotificationProvider> builder)
    {
        builder.ToTable("NotificationProviders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.ProviderType).HasConversion<int>().IsRequired();
        builder.Property(x => x.ConfigurationKey).HasMaxLength(150);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.Type).HasDatabaseName("IX_NotificationProviders_Type");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("IX_NotificationProviders_IsActive");
        builder.HasIndex(x => x.Priority).HasDatabaseName("IX_NotificationProviders_Priority");
    }
}
