using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder.HasKey(experience => experience.Id);

        builder.Property(experience => experience.Company)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(experience => experience.Position)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(experience => experience.Summary)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasMany(experience => experience.Highlights)
            .WithOne(highlight => highlight.WorkExperience)
            .HasForeignKey(highlight => highlight.WorkExperienceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
