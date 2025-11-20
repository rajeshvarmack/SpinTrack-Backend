namespace SpinTrack.Application.Features.BusinessHours.DTOs
{
    public class CreateBusinessHoursRequest
    {
        public Guid CompanyId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string ShiftName { get; set; } = "Default";
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsWorkingShift { get; set; } = true;
        public bool IsOvertimeEligible { get; set; } = false;
        public string? Remarks { get; set; }
    }
}