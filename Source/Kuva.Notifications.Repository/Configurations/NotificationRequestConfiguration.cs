using Kuva.Notifications.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Notifications.Repository.Configurations;

public sealed class NotificationRequestConfiguration : IEntityTypeConfiguration<NotificationRequest>
{
    public void Configure(EntityTypeBuilder<NotificationRequest> builder)
    {
        builder.ToTable("NotificationRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.TemplateCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ExternalReference).HasMaxLength(150);
        builder.Property(x => x.Source).HasMaxLength(100);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Priority).HasConversion<int>().IsRequired();
        builder.Property(x => x.VariablesJson).IsRequired();
        builder.Property(x => x.SubjectRendered).HasMaxLength(300);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.Template)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Recipients)
            .WithOne(x => x.NotificationRequest)
            .HasForeignKey(x => x.NotificationRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Attachments)
            .WithOne(x => x.NotificationRequest)
            .HasForeignKey(x => x.NotificationRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Attempts)
            .WithOne(x => x.NotificationRequest)
            .HasForeignKey(x => x.NotificationRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Events)
            .WithOne(x => x.NotificationRequest)
            .HasForeignKey(x => x.NotificationRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Type).HasDatabaseName("IX_NotificationRequests_Type");
        builder.HasIndex(x => x.TemplateCode).HasDatabaseName("IX_NotificationRequests_TemplateCode");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_NotificationRequests_Status");
        builder.HasIndex(x => x.ExternalReference).HasDatabaseName("IX_NotificationRequests_ExternalReference");
        builder.HasIndex(x => x.CreatedAtUtc).HasDatabaseName("IX_NotificationRequests_CreatedAtUtc");
        builder.HasIndex(x => new { x.Type, x.TemplateId, x.ExternalReference }).HasDatabaseName("IX_NotificationRequests_Type_TemplateId_ExternalReference");
    }
}
