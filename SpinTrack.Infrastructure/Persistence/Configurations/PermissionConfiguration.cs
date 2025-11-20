using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Permission;
using SpinTrack.Core.Enums;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permission", "auth");

            builder.HasKey(p => p.PermissionId);

            builder.Property(p => p.PermissionId).HasDefaultValueSql("NEWID()");

            builder.Property(p => p.SubModuleId).IsRequired();

            builder.Property(p => p.PermissionKey)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(p => p.PermissionKey).IsUnique();

            builder.Property(p => p.PermissionName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ModuleStatus.Active);

            builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(p => p.CreatedBy).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.ModifiedBy);
            builder.Property(p => p.ModifiedAt);

            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.CreatedAt);
            builder.HasIndex(p => p.IsDeleted);

            builder.HasQueryFilter(p => !p.IsDeleted);

            // FK to SubModule
            builder.HasOne<global::SpinTrack.Core.Entities.SubModule.SubModule>()
                .WithMany()
                .HasForeignKey(p => p.SubModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}