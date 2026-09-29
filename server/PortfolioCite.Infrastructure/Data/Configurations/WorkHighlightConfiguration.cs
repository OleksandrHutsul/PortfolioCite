using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class WorkHighlightConfiguration : IEntityTypeConfiguration<WorkHighlight>
{
    public void Configure(EntityTypeBuilder<WorkHighlight> builder)
    {
        builder.HasKey(highlight => highlight.Id);

        builder.Property(highlight => highlight.Text)
            .HasMaxLength(500)
            .IsRequired();
    }
}
