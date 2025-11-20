using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.BusinessHoliday
{
    public class BusinessHoliday : BaseEntity
    {
        public Guid BusinessHolidayId { get; set; }
        public Guid CompanyId { get; set; }
        public DateOnly HolidayDate { get; set; }
        public string HolidayName { get; set; } = string.Empty;
        public string HolidayType { get; set; } = "Public"; // Public, Company, Optional
        public Guid? CountryId { get; set; }
        public bool IsFullDay { get; set; } = true;
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation properties
        public Company.Company? Company { get; set; }
        public Country.Country? Country { get; set; }
    }
}