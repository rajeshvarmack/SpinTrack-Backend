namespace SpinTrack.Application.Features.ProductVersions.DTOs
{
    public class UpdateProductVersionRequest
    {
        public DateOnly ReleaseDate { get; set; }
        public string? ReleaseNotes { get; set; }
        public bool IsCurrent { get; set; }
    }
}