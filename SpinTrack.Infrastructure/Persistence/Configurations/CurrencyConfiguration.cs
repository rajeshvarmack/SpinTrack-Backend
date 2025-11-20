using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Currency;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currency", "master");

            builder.HasKey(c => c.CurrencyId);
            builder.Property(c => c.CurrencyId).HasDefaultValueSql("NEWID()");

            builder.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(10);
            builder.HasIndex(c => c.CurrencyCode).IsUnique();

            builder.Property(c => c.CurrencySymbol).HasMaxLength(20);
            builder.Property(c => c.DecimalPlaces).IsRequired().HasDefaultValue(2);
            builder.Property(c => c.IsDefault).IsRequired().HasDefaultValue(false);

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