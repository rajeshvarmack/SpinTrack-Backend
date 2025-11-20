namespace SpinTrack.Application.Features.BusinessHolidays.DTOs
{
    public class BusinessHolidayDto
    {
        public Guid BusinessHolidayId { get; set; }
        public Guid CompanyId { get; set; }
        public DateOnly HolidayDate { get; set; }
        public string HolidayName { get; set; } = string.Empty;
        public string HolidayType { get; set; } = string.Empty;
        public Guid? CountryId { get; set; }
        public bool IsFullDay { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}