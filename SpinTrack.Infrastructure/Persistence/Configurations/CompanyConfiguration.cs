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

            // Foreign Keys with Navigation Properties
            builder.HasOne(c => c.Country)
                .WithMany()
                .HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Currency)
                .WithMany()
                .HasForeignKey(c => c.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.TimeZone)
                .WithMany()
                .HasForeignKey(c => c.TimeZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.DateFormat)
                .WithMany()
                .HasForeignKey(c => c.DateFormatId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // One-to-many relationships
            builder.HasMany(c => c.BusinessDays)
                .WithOne(bd => bd.Company)
                .HasForeignKey(bd => bd.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.BusinessHours)
                .WithOne(bh => bh.Company)
                .HasForeignKey(bh => bh.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.BusinessHolidays)
                .WithOne(bh => bh.Company)
                .HasForeignKey(bh => bh.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}