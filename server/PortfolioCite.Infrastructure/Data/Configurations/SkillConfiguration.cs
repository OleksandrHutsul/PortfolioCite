using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(skill => skill.Id);

        builder.Property(skill => skill.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(skill => skill.Description)
            .HasMaxLength(600)
            .IsRequired();

        builder.Property(skill => skill.Badge)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(skill => skill.IconName)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(skill => skill.AccentColor)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(skill => new
        {
            skill.SkillCategoryId,
            skill.Name
        })
        .IsUnique();
    }
}
