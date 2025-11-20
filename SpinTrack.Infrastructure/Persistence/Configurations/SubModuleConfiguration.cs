using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.SubModule;
using SpinTrack.Core.Enums;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class SubModuleConfiguration : IEntityTypeConfiguration<SubModule>
    {
        public void Configure(EntityTypeBuilder<SubModule> builder)
        {
            builder.ToTable("SubModule", "auth");

            builder.HasKey(s => s.SubModuleId);

            builder.Property(s => s.SubModuleId).HasDefaultValueSql("NEWID()");

            builder.Property(s => s.ModuleId).IsRequired();

            builder.Property(s => s.SubModuleKey)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(s => s.SubModuleKey).IsUnique();

            builder.Property(s => s.SubModuleName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ModuleStatus.Active);

            builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(s => s.CreatedBy).IsRequired();
            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.ModifiedBy);
            builder.Property(s => s.ModifiedAt);

            builder.HasIndex(s => s.Status);
            builder.HasIndex(s => s.CreatedAt);
            builder.HasIndex(s => s.IsDeleted);

            builder.HasQueryFilter(s => !s.IsDeleted);

            // Foreign key relationship to Module - use fully qualified type in HasOne overload
            builder.HasOne<global::SpinTrack.Core.Entities.Module.Module>()
                .WithMany()
                .HasForeignKey(s => s.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}