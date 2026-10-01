using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ProjectImageConfiguration : IEntityTypeConfiguration<ProjectImage>
{
    public void Configure(EntityTypeBuilder<ProjectImage> builder)
    {
        builder.HasKey(image => image.Id);

        builder.Property(image => image.FileName)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(image => image.ContentType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(image => image.Content)
            .IsRequired();

        builder.HasIndex(image => image.ProjectId)
            .IsUnique();

        builder.HasOne(image => image.Project)
            .WithOne(project => project.Image)
            .HasForeignKey<ProjectImage>(image => image.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
