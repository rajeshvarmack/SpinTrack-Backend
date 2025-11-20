namespace SpinTrack.Application.Features.BusinessHolidays.DTOs
{
    public class CreateBusinessHolidayRequest
    {
        public Guid CompanyId { get; set; }
        public DateOnly HolidayDate { get; set; }
        public string HolidayName { get; set; } = string.Empty;
        public string HolidayType { get; set; } = "Public";
        public Guid? CountryId { get; set; }
        public bool IsFullDay { get; set; } = true;
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}