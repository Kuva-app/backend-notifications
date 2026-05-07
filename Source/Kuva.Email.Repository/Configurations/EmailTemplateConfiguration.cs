using Kuva.Email.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Email.Repository.Configurations;

public sealed class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.ToTable("EmailTemplates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.SubjectTemplate).HasMaxLength(300).IsRequired();
        builder.Property(x => x.HtmlBodyTemplate).IsRequired();
        builder.Property(x => x.RequiredVariablesJson).IsRequired();
        builder.Property(x => x.Language).HasMaxLength(10).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => x.Code).HasDatabaseName("IX_EmailTemplates_Code");
        builder.HasIndex(x => new { x.Code, x.IsActive }).HasDatabaseName("IX_EmailTemplates_Code_IsActive");
        builder.HasIndex(x => new { x.Code, x.Version }).IsUnique();
    }
}
