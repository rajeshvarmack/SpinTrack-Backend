using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.TimeZone
{
    public class TimeZoneEntity : BaseEntity
    {
        public Guid TimeZoneId { get; set; }
        public string TimeZoneName { get; set; } = string.Empty;
        public string? GMTOffset { get; set; }
        public bool SupportsDST { get; set; }

        public bool IsDeleted { get; set; }
    }
}