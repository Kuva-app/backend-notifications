using Kuva.Notifications.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Notifications.Repository.Configurations;

public sealed class NotificationSendAttemptConfiguration : IEntityTypeConfiguration<NotificationSendAttempt>
{
    public void Configure(EntityTypeBuilder<NotificationSendAttempt> builder)
    {
        builder.ToTable("NotificationSendAttempts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(200);
        builder.Property(x => x.ErrorCode).HasMaxLength(100);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.Property(x => x.StartedAtUtc).IsRequired();

        builder.HasOne(x => x.NotificationProvider)
            .WithMany(x => x.Attempts)
            .HasForeignKey(x => x.NotificationProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.NotificationRequestId).HasDatabaseName("IX_NotificationSendAttempts_NotificationRequestId");
        builder.HasIndex(x => x.Success).HasDatabaseName("IX_NotificationSendAttempts_Success");
        builder.HasIndex(x => x.StartedAtUtc).HasDatabaseName("IX_NotificationSendAttempts_StartedAtUtc");
    }
}
