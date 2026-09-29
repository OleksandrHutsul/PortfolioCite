using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(project => project.ShortDescription)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(project => project.GitHubUrl)
            .HasMaxLength(500);

        builder.Property(project => project.LiveUrl)
            .HasMaxLength(500);

        builder.Property(project => project.ImageUrl)
            .HasMaxLength(500);

        builder.Property(project => project.CreatedAt)
            .IsRequired();

        builder.Property(project => project.UpdatedAt)
            .IsRequired();

        builder.HasIndex(project => new
        {
            project.IsPublished,
            project.DisplayOrder
        });
    }
}
