namespace SpinTrack.Application.Features.TimeZones.DTOs
{
    public class CreateTimeZoneRequest
    {
        public string TimeZoneName { get; set; } = string.Empty;
        public string? GMTOffset { get; set; }
        public bool SupportsDST { get; set; }
    }
}