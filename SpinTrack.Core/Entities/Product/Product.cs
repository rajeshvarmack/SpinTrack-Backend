using SpinTrack.Core.Entities.Common;

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
    }
}