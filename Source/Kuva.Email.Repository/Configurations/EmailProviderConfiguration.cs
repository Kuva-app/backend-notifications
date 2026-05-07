using Kuva.Email.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Email.Repository.Configurations;

public sealed class EmailProviderConfiguration : IEntityTypeConfiguration<EmailProvider>
{
    public void Configure(EntityTypeBuilder<EmailProvider> builder)
    {
        builder.ToTable("EmailProviders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProviderType).HasConversion<int>().IsRequired();
        builder.Property(x => x.ConfigurationKey).HasMaxLength(150);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.IsActive).HasDatabaseName("IX_EmailProviders_IsActive");
        builder.HasIndex(x => x.Priority).HasDatabaseName("IX_EmailProviders_Priority");
    }
}
