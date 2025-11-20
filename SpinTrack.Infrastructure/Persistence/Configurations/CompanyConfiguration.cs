using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Company;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company", "master");

            builder.HasKey(c => c.CompanyId);
            builder.Property(c => c.CompanyId).HasDefaultValueSql("NEWID()");

            builder.Property(c => c.CompanyCode).IsRequired().HasMaxLength(50);
            builder.HasIndex(c => c.CompanyCode).IsUnique();

            builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(150);

            builder.Property(c => c.CountryId).IsRequired();
            builder.Property(c => c.CurrencyId).IsRequired();
            builder.Property(c => c.TimeZoneId).IsRequired();
            builder.Property(c => c.DateFormatId);

            builder.Property(c => c.Website).HasMaxLength(200);
            builder.Property(c => c.Address).HasMaxLength(500);
            builder.Property(c => c.LogoUrl).HasMaxLength(256);

            builder.Property(c => c.FiscalYearStartMonth).IsRequired().HasDefaultValue(1);

            builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(c => c.CreatedBy).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.ModifiedBy);
            builder.Property(c => c.ModifiedAt);

            builder.HasIndex(c => c.CreatedAt);
            builder.HasIndex(c => c.IsDeleted);

            builder.HasQueryFilter(c => !c.IsDeleted);

            // FKs will rely on existing master tables
        }
    }
}