using Kuva.Email.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Email.Repository.Configurations;

public sealed class EmailEventConfiguration : IEntityTypeConfiguration<EmailEvent>
{
    public void Configure(EntityTypeBuilder<EmailEvent> builder)
    {
        builder.ToTable("EmailEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasConversion<int>().IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasIndex(x => x.EmailRequestId).HasDatabaseName("IX_EmailEvents_EmailRequestId");
        builder.HasIndex(x => x.EventType).HasDatabaseName("IX_EmailEvents_EventType");
        builder.HasIndex(x => x.CreatedAtUtc).HasDatabaseName("IX_EmailEvents_CreatedAtUtc");
    }
}
