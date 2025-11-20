namespace SpinTrack.Application.Features.BusinessHours.DTOs
{
    public class UpdateBusinessHoursRequest
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsWorkingShift { get; set; } = true;
        public bool IsOvertimeEligible { get; set; } = false;
        public string? Remarks { get; set; }
    }
}