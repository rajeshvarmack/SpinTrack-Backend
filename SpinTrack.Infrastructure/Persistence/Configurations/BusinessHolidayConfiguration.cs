using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.BusinessHoliday;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class BusinessHolidayConfiguration : IEntityTypeConfiguration<BusinessHoliday>
    {
        public void Configure(EntityTypeBuilder<BusinessHoliday> builder)
        {
            builder.ToTable("BusinessHoliday", "master");

            builder.HasKey(bh => bh.BusinessHolidayId);
            builder.Property(bh => bh.BusinessHolidayId).HasDefaultValueSql("NEWID()");

            builder.Property(bh => bh.CompanyId).IsRequired();
            builder.Property(bh => bh.HolidayDate).IsRequired();
            builder.Property(bh => bh.HolidayName).IsRequired().HasMaxLength(150);
            builder.Property(bh => bh.HolidayType).IsRequired().HasMaxLength(50);
            builder.HasIndex(bh => new { bh.CompanyId, bh.HolidayDate }).IsUnique();

            builder.Property(bh => bh.CountryId);
            builder.Property(bh => bh.IsFullDay).IsRequired().HasDefaultValue(true);
            builder.Property(bh => bh.StartTime);
            builder.Property(bh => bh.EndTime);

            builder.Property(bh => bh.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(bh => bh.CreatedBy).IsRequired();
            builder.Property(bh => bh.CreatedAt).IsRequired();
            builder.Property(bh => bh.ModifiedBy);
            builder.Property(bh => bh.ModifiedAt);

            builder.HasIndex(bh => bh.CreatedAt);
            builder.HasIndex(bh => bh.IsDeleted);

            builder.HasQueryFilter(bh => !bh.IsDeleted);
        }
    }
}