using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.DateFormat;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class DateFormatConfiguration : IEntityTypeConfiguration<DateFormat>
    {
        public void Configure(EntityTypeBuilder<DateFormat> builder)
        {
            builder.ToTable("DateFormat", "master");

            builder.HasKey(df => df.DateFormatId);
            builder.Property(df => df.DateFormatId).HasDefaultValueSql("NEWID()");

            builder.Property(df => df.FormatString).IsRequired().HasMaxLength(50);
            builder.HasIndex(df => df.FormatString).IsUnique();

            builder.Property(df => df.Description).HasMaxLength(150);
            builder.Property(df => df.IsDefault).IsRequired().HasDefaultValue(false);

            builder.Property(df => df.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(df => df.CreatedBy).IsRequired();
            builder.Property(df => df.CreatedAt).IsRequired();
            builder.Property(df => df.ModifiedBy);
            builder.Property(df => df.ModifiedAt);

            builder.HasIndex(df => df.CreatedAt);
            builder.HasIndex(df => df.IsDeleted);

            builder.HasQueryFilter(df => !df.IsDeleted);
        }
    }
}