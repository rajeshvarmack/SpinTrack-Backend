using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.ProductVersion
{
    public class ProductVersion : BaseEntity
    {
        public Guid ProductVersionId { get; set; }
        public Guid ProductId { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public string? ReleaseNotes { get; set; }
        public bool IsCurrent { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation property
        public Product.Product? Product { get; set; }
    }
}