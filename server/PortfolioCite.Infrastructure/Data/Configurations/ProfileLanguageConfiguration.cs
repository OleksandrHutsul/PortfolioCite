using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ProfileLanguageConfiguration : IEntityTypeConfiguration<ProfileLanguage>
{
    public void Configure(EntityTypeBuilder<ProfileLanguage> builder)
    {
        builder.HasKey(language => language.Id);

        builder.Property(language => language.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(language => language.Proficiency)
            .HasMaxLength(16)
            .IsRequired();

        builder.HasIndex(language => language.ProfileId);
    }
}
