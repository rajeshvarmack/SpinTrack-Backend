namespace SpinTrack.Application.Features.BusinessDays.DTOs
{
    public class BusinessDayDto
    {
        public Guid BusinessDayId { get; set; }
        public Guid CompanyId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public bool IsWorkingDay { get; set; }
        public bool IsWeekend { get; set; }
        public string? Remarks { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}