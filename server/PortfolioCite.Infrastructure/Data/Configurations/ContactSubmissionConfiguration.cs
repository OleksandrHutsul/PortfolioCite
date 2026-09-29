using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ContactSubmissionConfiguration : IEntityTypeConfiguration<ContactSubmission>
{
    public void Configure(EntityTypeBuilder<ContactSubmission> builder)
    {
        builder.HasKey(submission => submission.Id);

        builder.Property(submission => submission.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(submission => submission.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(submission => submission.Subject)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(submission => submission.Message)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(submission => submission.CreatedAt)
            .IsRequired();

        builder.HasIndex(submission => new
        {
            submission.IsRead,
            submission.CreatedAt
        });
    }
}
