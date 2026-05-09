using Kuva.Notifications.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Notifications.Repository.Configurations;

public sealed class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("NotificationTemplates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SubjectTemplate).HasMaxLength(300).IsRequired(false);
        builder.Property(x => x.HtmlBodyTemplate).IsRequired();
        builder.Property(x => x.RequiredVariablesJson).IsRequired();
        builder.Property(x => x.Language).HasMaxLength(10).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => x.Type).HasDatabaseName("IX_NotificationTemplates_Type");
        builder.HasIndex(x => x.Code).HasDatabaseName("IX_NotificationTemplates_Code");
        builder.HasIndex(x => new { x.Type, x.Code, x.IsActive }).HasDatabaseName("IX_NotificationTemplates_Type_Code_IsActive");
        builder.HasIndex(x => new { x.Type, x.Code, x.Version }).IsUnique();
    }
}
