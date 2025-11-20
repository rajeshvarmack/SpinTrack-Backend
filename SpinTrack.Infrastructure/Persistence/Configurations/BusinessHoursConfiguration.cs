using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.BusinessHours;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class BusinessHoursConfiguration : IEntityTypeConfiguration<BusinessHour>
    {
        public void Configure(EntityTypeBuilder<BusinessHour> builder)
        {
            builder.ToTable("BusinessHours", "master");

            builder.HasKey(bh => bh.BusinessHoursId);
            builder.Property(bh => bh.BusinessHoursId).HasDefaultValueSql("NEWID()");

            builder.Property(bh => bh.CompanyId).IsRequired();
            builder.Property(bh => bh.DayOfWeek).IsRequired().HasMaxLength(20);
            builder.Property(bh => bh.ShiftName).IsRequired().HasMaxLength(50);
            builder.HasIndex(bh => new { bh.CompanyId, bh.DayOfWeek, bh.ShiftName }).IsUnique();

            builder.Property(bh => bh.StartTime).IsRequired();
            builder.Property(bh => bh.EndTime).IsRequired();
            builder.Property(bh => bh.IsWorkingShift).IsRequired().HasDefaultValue(true);
            builder.Property(bh => bh.IsOvertimeEligible).IsRequired().HasDefaultValue(false);
            builder.Property(bh => bh.Remarks).HasMaxLength(250);

            builder.Property(bh => bh.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(bh => bh.CreatedBy).IsRequired();
            builder.Property(bh => bh.CreatedAt).IsRequired();
            builder.Property(bh => bh.ModifiedBy);
            builder.Property(bh => bh.ModifiedAt);

            builder.HasIndex(bh => bh.CreatedAt);
            builder.HasIndex(bh => bh.IsDeleted);

            builder.HasQueryFilter(bh => !bh.IsDeleted);

            builder.HasCheckConstraint("CHK_BusinessHours_Time", "[EndTime] > [StartTime]");
        }
    }
}