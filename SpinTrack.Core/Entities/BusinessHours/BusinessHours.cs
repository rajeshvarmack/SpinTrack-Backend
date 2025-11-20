using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.BusinessHours
{
    public class BusinessHour : BaseEntity
    {
        public Guid BusinessHoursId { get; set; }
        public Guid CompanyId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string ShiftName { get; set; } = "Default";
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsWorkingShift { get; set; } = true;
        public bool IsOvertimeEligible { get; set; } = false;
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; }
    }
}