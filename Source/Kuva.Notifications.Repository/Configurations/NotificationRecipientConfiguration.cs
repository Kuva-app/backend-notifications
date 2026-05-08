using Kuva.Notifications.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Notifications.Repository.Configurations;

public sealed class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
{
    public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        builder.ToTable("NotificationRecipients");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Address).HasMaxLength(320).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150);
        builder.Property(x => x.Role).HasMaxLength(30).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.NotificationRequestId).HasDatabaseName("IX_NotificationRecipients_NotificationRequestId");
        builder.HasIndex(x => x.Address).HasDatabaseName("IX_NotificationRecipients_Address");
    }
}
