using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.BusinessDay
{
    public class BusinessDay : BaseEntity
    {
        public Guid BusinessDayId { get; set; }
        public Guid CompanyId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty; // e.g., Sunday, Monday
        public bool IsWorkingDay { get; set; } = true;
        public bool IsWeekend { get; set; } = false;
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; }
    }
}