namespace SpinTrack.Application.Features.ProductVersions.DTOs
{
    public class ProductVersionDetailDto
    {
        public Guid ProductVersionId { get; set; }
        public Guid ProductId { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public string? ReleaseNotes { get; set; }
        public bool IsCurrent { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}