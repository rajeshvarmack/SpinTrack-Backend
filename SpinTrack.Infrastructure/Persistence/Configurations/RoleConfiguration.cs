using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Role;
using SpinTrack.Core.Enums;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity configuration for Role entity
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role", "auth");

            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.RoleId)
                .HasDefaultValueSql("NEWID()");

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(r => r.RoleName)
                .IsUnique();

            builder.Property(r => r.Description)
                .HasMaxLength(250);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(RoleStatus.Active);

            // Soft Delete
            builder.Property(r => r.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.DeletedAt);
            builder.Property(r => r.DeletedBy);

            // Audit Fields
            builder.Property(r => r.CreatedBy)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.ModifiedBy);
            builder.Property(r => r.ModifiedAt);

            // Indexes
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.CreatedAt);
            builder.HasIndex(r => r.IsDeleted);

            // Query filter for soft delete
            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}