using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.FullName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(profile => profile.Role)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(profile => profile.Location)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(profile => profile.Summary)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(profile => profile.CurrentFocus)
            .HasMaxLength(1200)
            .IsRequired();

        builder.Property(profile => profile.Languages)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(profile => profile.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(profile => profile.AvatarUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(profile => profile.ResumeUrl)
            .HasMaxLength(500);

        builder.Property(profile => profile.UpdatedAt)
            .IsRequired();
    }
}
