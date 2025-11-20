using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.DateFormat
{
    public class DateFormat : BaseEntity
    {
        public Guid DateFormatId { get; set; }
        public string FormatString { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; }
    }
}