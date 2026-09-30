using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data.Configurations;

public class ProfileFileConfiguration : IEntityTypeConfiguration<ProfileFile>
{
    public void Configure(EntityTypeBuilder<ProfileFile> builder)
    {
        builder.HasKey(file => file.Id);

        builder.Property(file => file.Kind)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(file => file.FileName)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(file => file.ContentType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(file => file.Content)
            .IsRequired();

        builder.HasIndex(file => new { file.ProfileId, file.Kind })
            .IsUnique();

        builder.HasOne(file => file.Profile)
            .WithMany(profile => profile.Files)
            .HasForeignKey(file => file.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
