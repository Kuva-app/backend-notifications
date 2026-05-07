using Kuva.Email.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kuva.Email.Repository.Configurations;

public sealed class EmailSendAttemptConfiguration : IEntityTypeConfiguration<EmailSendAttempt>
{
    public void Configure(EntityTypeBuilder<EmailSendAttempt> builder)
    {
        builder.ToTable("EmailSendAttempts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProviderMessageId).HasMaxLength(200);
        builder.Property(x => x.ErrorCode).HasMaxLength(100);
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.Property(x => x.StartedAtUtc).IsRequired();

        builder.HasOne(x => x.EmailProvider)
            .WithMany(x => x.Attempts)
            .HasForeignKey(x => x.EmailProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.EmailRequestId).HasDatabaseName("IX_EmailSendAttempts_EmailRequestId");
        builder.HasIndex(x => x.Success).HasDatabaseName("IX_EmailSendAttempts_Success");
        builder.HasIndex(x => x.StartedAtUtc).HasDatabaseName("IX_EmailSendAttempts_StartedAtUtc");
    }
}
