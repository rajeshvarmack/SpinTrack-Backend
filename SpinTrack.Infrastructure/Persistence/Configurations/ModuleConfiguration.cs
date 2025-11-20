using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Module;
using SpinTrack.Core.Enums;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("Module", "auth");

            builder.HasKey(m => m.ModuleId);

            builder.Property(m => m.ModuleId)
                .HasDefaultValueSql("NEWID()");

            builder.Property(m => m.ModuleKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(m => m.ModuleKey)
                .IsUnique();

            builder.Property(m => m.ModuleName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(m => m.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ModuleStatus.Active);

            builder.Property(m => m.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.CreatedBy)
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.ModifiedBy);
            builder.Property(m => m.ModifiedAt);

            builder.HasIndex(m => m.Status);
            builder.HasIndex(m => m.CreatedAt);
            builder.HasIndex(m => m.IsDeleted);

            builder.HasQueryFilter(m => !m.IsDeleted);

            // One-to-many relationship with SubModules
            builder.HasMany(m => m.SubModules)
                .WithOne(s => s.Module)
                .HasForeignKey(s => s.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}