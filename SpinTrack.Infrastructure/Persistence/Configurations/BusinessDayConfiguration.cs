using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.BusinessDay;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class BusinessDayConfiguration : IEntityTypeConfiguration<BusinessDay>
    {
        public void Configure(EntityTypeBuilder<BusinessDay> builder)
        {
            builder.ToTable("BusinessDay", "master");

            builder.HasKey(bd => bd.BusinessDayId);
            builder.Property(bd => bd.BusinessDayId).HasDefaultValueSql("NEWID()");

            builder.Property(bd => bd.CompanyId).IsRequired();
            builder.Property(bd => bd.DayOfWeek).IsRequired().HasMaxLength(20);
            builder.HasIndex(bd => new { bd.CompanyId, bd.DayOfWeek }).IsUnique();

            builder.Property(bd => bd.IsWorkingDay).IsRequired().HasDefaultValue(true);
            builder.Property(bd => bd.IsWeekend).IsRequired().HasDefaultValue(false);
            builder.Property(bd => bd.Remarks).HasMaxLength(250);

            builder.Property(bd => bd.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(bd => bd.CreatedBy).IsRequired();
            builder.Property(bd => bd.CreatedAt).IsRequired();
            builder.Property(bd => bd.ModifiedBy);
            builder.Property(bd => bd.ModifiedAt);

            builder.HasIndex(bd => bd.CreatedAt);
            builder.HasIndex(bd => bd.IsDeleted);

            builder.HasQueryFilter(bd => !bd.IsDeleted);
        }
    }
}