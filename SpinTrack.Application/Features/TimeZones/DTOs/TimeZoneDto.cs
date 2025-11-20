namespace SpinTrack.Application.Features.TimeZones.DTOs
{
    public class TimeZoneDto
    {
        public Guid TimeZoneId { get; set; }
        public string TimeZoneName { get; set; } = string.Empty;
        public string? GMTOffset { get; set; }
        public bool SupportsDST { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}