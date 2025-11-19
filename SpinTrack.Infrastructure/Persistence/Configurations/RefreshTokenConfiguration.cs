using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Auth;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity configuration for RefreshToken entity
    /// </summary>
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken", "auth");

            builder.HasKey(rt => rt.RefreshTokenId);

            builder.Property(rt => rt.RefreshTokenId)
                .HasDefaultValueSql("NEWID()");

            builder.Property(rt => rt.UserId)
                .IsRequired();

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(rt => rt.Token)
                .IsUnique();

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            builder.Property(rt => rt.RevokedAt);

            // Indexes for performance
            builder.HasIndex(rt => rt.UserId);
            builder.HasIndex(rt => rt.ExpiresAt);
            builder.HasIndex(rt => rt.RevokedAt);
        }
    }
}
