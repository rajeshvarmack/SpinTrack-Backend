using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Auth;
using SpinTrack.Core.Enums;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity configuration for User entity
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User", "auth");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                .HasDefaultValueSql("NEWID()");

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.MiddleName)
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .HasMaxLength(50);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.NationalId)
                .HasMaxLength(50);

            builder.Property(u => u.Gender)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(u => u.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(u => u.Nationality)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.JobTitle)
                .HasMaxLength(50);

            builder.Property(u => u.ProfilePicturePath)
                .HasMaxLength(256);

            builder.Property(u => u.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(UserStatus.Active);

            // Account Lockout
            builder.Property(u => u.FailedLoginAttempts)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(u => u.LockoutEnd);

            // Soft Delete
            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.DeletedAt);

            builder.Property(u => u.DeletedBy);

            // Audit Fields
            builder.Property(u => u.CreatedBy)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.ModifiedBy);

            builder.Property(u => u.ModifiedAt);

            // Indexes for performance
            builder.HasIndex(u => u.Status);
            builder.HasIndex(u => u.CreatedAt);
            builder.HasIndex(u => u.IsDeleted);

            // Query filter for soft delete
            builder.HasQueryFilter(u => !u.IsDeleted);

            // Navigation
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
