namespace SpinTrack.Application.Features.DateFormats.DTOs
{
    public class DateFormatDetailDto
    {
        public Guid DateFormatId { get; set; }
        public string FormatString { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}