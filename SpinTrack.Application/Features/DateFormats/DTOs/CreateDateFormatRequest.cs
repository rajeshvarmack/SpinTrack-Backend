namespace SpinTrack.Application.Features.DateFormats.DTOs
{
    public class CreateDateFormatRequest
    {
        public string FormatString { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
    }
}