namespace SpinTrack.Application.Features.BusinessHours.DTOs
{
    public class BusinessHoursDetailDto
    {
        public Guid BusinessHoursId { get; set; }
        public Guid CompanyId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsWorkingShift { get; set; }
        public bool IsOvertimeEligible { get; set; }
        public string? Remarks { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}