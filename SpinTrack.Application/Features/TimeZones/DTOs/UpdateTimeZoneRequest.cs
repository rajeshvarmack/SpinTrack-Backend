namespace SpinTrack.Application.Features.TimeZones.DTOs
{
    public class UpdateTimeZoneRequest
    {
        public string? GMTOffset { get; set; }
        public bool SupportsDST { get; set; }
    }
}