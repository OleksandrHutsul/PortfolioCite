using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class SkillCategoryConfiguration : IEntityTypeConfiguration<SkillCategory>
{
    public void Configure(EntityTypeBuilder<SkillCategory> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(category => category.Name)
            .IsUnique();

        builder.HasMany(category => category.Skills)
            .WithOne(skill => skill.SkillCategory)
            .HasForeignKey(skill => skill.SkillCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
