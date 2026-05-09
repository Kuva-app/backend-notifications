using Kuva.Notifications.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Notifications.Repository.Configurations;

public sealed class NotificationEventConfiguration : IEntityTypeConfiguration<NotificationEvent>
{
    public void Configure(EntityTypeBuilder<NotificationEvent> builder)
    {
        builder.ToTable("NotificationEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.NotificationRequestId).HasDatabaseName("IX_NotificationEvents_NotificationRequestId");
        builder.HasIndex(x => x.EventType).HasDatabaseName("IX_NotificationEvents_EventType");
        builder.HasIndex(x => x.CreatedAtUtc).HasDatabaseName("IX_NotificationEvents_CreatedAtUtc");
    }
}
