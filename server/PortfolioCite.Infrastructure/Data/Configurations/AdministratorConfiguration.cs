using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.HasKey(administrator => administrator.Id);

        builder.Property(administrator => administrator.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(administrator => administrator.NormalizedEmail)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(administrator => administrator.PasswordHash)
            .IsRequired();

        builder.Property(administrator => administrator.CreatedAt)
            .IsRequired();

        builder.HasIndex(administrator => administrator.NormalizedEmail)
            .IsUnique();
    }
}
