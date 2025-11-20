using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.ProductVersion;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class ProductVersionConfiguration : IEntityTypeConfiguration<ProductVersion>
    {
        public void Configure(EntityTypeBuilder<ProductVersion> builder)
        {
            builder.ToTable("ProductVersion", "master");

            builder.HasKey(pv => pv.ProductVersionId);
            builder.Property(pv => pv.ProductVersionId).HasDefaultValueSql("NEWID()");

            builder.Property(pv => pv.ProductId).IsRequired();
            builder.Property(pv => pv.VersionNumber).IsRequired().HasMaxLength(20);
            builder.HasIndex(pv => new { pv.ProductId, pv.VersionNumber }).IsUnique();

            builder.Property(pv => pv.ReleaseDate).IsRequired();
            builder.Property(pv => pv.ReleaseNotes);
            builder.Property(pv => pv.IsCurrent).IsRequired().HasDefaultValue(false);

            builder.Property(pv => pv.CreatedBy).IsRequired();
            builder.Property(pv => pv.CreatedAt).IsRequired();

            builder.HasIndex(pv => pv.CreatedAt);
            builder.HasIndex(pv => pv.IsCurrent);

            builder.HasQueryFilter(pv => !pv.IsDeleted);

            // Foreign Key configured from Product side
        }
    }
}