using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ContactLinkConfiguration : IEntityTypeConfiguration<ContactLink>
{
    public void Configure(EntityTypeBuilder<ContactLink> builder)
    {
        builder.HasKey(link => link.Id);

        builder.Property(link => link.Label)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(link => link.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(link => link.IconName)
            .HasMaxLength(40)
            .IsRequired();
    }
}
