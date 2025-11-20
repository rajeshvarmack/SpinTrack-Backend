namespace SpinTrack.Application.Features.ProductVersions.DTOs
{
    public class CreateProductVersionRequest
    {
        public Guid ProductId { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public string? ReleaseNotes { get; set; }
        public bool IsCurrent { get; set; }
    }
}