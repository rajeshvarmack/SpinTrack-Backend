using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpinTrack.Core.Entities.Product;

namespace SpinTrack.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product", "master");

            builder.HasKey(p => p.ProductId);
            builder.Property(p => p.ProductId).HasDefaultValueSql("NEWID()");

            builder.Property(p => p.ProductCode).IsRequired().HasMaxLength(50);
            builder.HasIndex(p => p.ProductCode).IsUnique();

            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(500);

            builder.Property(p => p.CurrentVersion).IsRequired().HasMaxLength(20);
            builder.Property(p => p.ReleaseDate);
            builder.Property(p => p.TechnologyStack).HasMaxLength(250);

            builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.Property(p => p.CreatedBy).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.ModifiedBy);
            builder.Property(p => p.ModifiedAt);

            builder.HasIndex(p => p.CreatedAt);
            builder.HasIndex(p => p.IsDeleted);

            builder.HasQueryFilter(p => !p.IsDeleted);

            // One-to-many relationship with ProductVersions
            builder.HasMany(p => p.ProductVersions)
                .WithOne(pv => pv.Product)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}