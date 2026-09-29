using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(certificate => certificate.Id);

        builder.Property(certificate => certificate.Name)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(certificate => certificate.Issuer)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(certificate => certificate.CredentialUrl)
            .HasMaxLength(500)
            .IsRequired();
    }
}
