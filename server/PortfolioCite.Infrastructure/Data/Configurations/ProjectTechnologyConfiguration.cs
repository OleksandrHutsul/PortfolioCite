using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ProjectTechnologyConfiguration : IEntityTypeConfiguration<ProjectTechnology>
{
    public void Configure(EntityTypeBuilder<ProjectTechnology> builder)
    {
        builder.HasKey(item => new
        {
            item.ProjectId,
            item.TechnologyId
        });

        builder.HasOne(item => item.Project)
            .WithMany(project => project.ProjectTechnologies)
            .HasForeignKey(item => item.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Technology)
            .WithMany(technology => technology.ProjectTechnologies)
            .HasForeignKey(item => item.TechnologyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}