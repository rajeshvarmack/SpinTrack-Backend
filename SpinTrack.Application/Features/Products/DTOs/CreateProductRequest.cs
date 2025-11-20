namespace SpinTrack.Application.Features.Products.DTOs
{
    public class CreateProductRequest
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CurrentVersion { get; set; } = string.Empty;
        public DateOnly? ReleaseDate { get; set; }
        public string? TechnologyStack { get; set; }
    }
}