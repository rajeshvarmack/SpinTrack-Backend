using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Entities.ProductVersion;

namespace SpinTrack.Core.Entities.Product
{
    public class Product : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CurrentVersion { get; set; } = string.Empty;
        public DateOnly? ReleaseDate { get; set; }
        public string? TechnologyStack { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation property
        public ICollection<SpinTrack.Core.Entities.ProductVersion.ProductVersion> ProductVersions { get; set; } = new List<SpinTrack.Core.Entities.ProductVersion.ProductVersion>();
    }
}