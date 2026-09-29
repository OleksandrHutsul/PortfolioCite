using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class TechnologyConfiguration : IEntityTypeConfiguration<Technology>
{
    public void Configure(EntityTypeBuilder<Technology> builder)
    {
        builder.HasKey(technology => technology.Id);

        builder.Property(technology => technology.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(technology => technology.NormalizedName)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(technology => technology.NormalizedName)
            .IsUnique();
    }
}
