namespace SpinTrack.Application.Features.BusinessDays.DTOs
{
    public class CreateBusinessDayRequest
    {
        public Guid CompanyId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public bool IsWorkingDay { get; set; } = true;
        public bool IsWeekend { get; set; } = false;
        public string? Remarks { get; set; }
    }
}