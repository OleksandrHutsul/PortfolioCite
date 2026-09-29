using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.HasKey(education => education.Id);

        builder.Property(education => education.Institution)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(education => education.Degree)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(education => education.FieldOfStudy)
            .HasMaxLength(160);

        builder.Property(education => education.Description)
            .HasMaxLength(1200);
    }
}
