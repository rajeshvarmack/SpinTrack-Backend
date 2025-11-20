using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.TimeZone;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class TimeZoneConfiguration : IEntityTypeConfiguration<TimeZoneEntity>
    {
        public void Configure(EntityTypeBuilder<TimeZoneEntity> builder)
        {
            builder.ToTable("TimeZone", "master");

            builder.HasKey(tz => tz.TimeZoneId);
            builder.Property(tz => tz.TimeZoneId).HasDefaultValueSql("NEWID()");

            builder.Property(tz => tz.TimeZoneName).IsRequired().HasMaxLength(100);
            builder.HasIndex(tz => tz.TimeZoneName).IsUnique();

            builder.Property(tz => tz.GMTOffset).HasMaxLength(10);
            builder.Property(tz => tz.SupportsDST).IsRequired().HasDefaultValue(false);

            builder.Property(tz => tz.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(tz => tz.CreatedBy).IsRequired();
            builder.Property(tz => tz.CreatedAt).IsRequired();
            builder.Property(tz => tz.ModifiedBy);
            builder.Property(tz => tz.ModifiedAt);

            builder.HasIndex(tz => tz.CreatedAt);
            builder.HasIndex(tz => tz.IsDeleted);

            builder.HasQueryFilter(tz => !tz.IsDeleted);
        }
    }
}