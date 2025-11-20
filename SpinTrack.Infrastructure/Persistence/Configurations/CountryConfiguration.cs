using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Country;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Country", "master");

            builder.HasKey(c => c.CountryId);
            builder.Property(c => c.CountryId).HasDefaultValueSql("NEWID()");

            builder.Property(c => c.CountryCodeISO2).IsRequired().HasMaxLength(2);
            builder.HasIndex(c => c.CountryCodeISO2).IsUnique();

            builder.Property(c => c.CountryCodeISO3).IsRequired().HasMaxLength(3);

            builder.Property(c => c.CountryName).IsRequired().HasMaxLength(150);
            builder.HasIndex(c => c.CountryName).IsUnique();

            builder.Property(c => c.PhoneCode).HasMaxLength(10);
            builder.Property(c => c.Continent).HasMaxLength(50);

            builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(c => c.CreatedBy).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.ModifiedBy);
            builder.Property(c => c.ModifiedAt);

            builder.HasIndex(c => c.CreatedAt);
            builder.HasIndex(c => c.IsDeleted);

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}