using SpinTrack.Core.Entities.Common;
using System.Collections.Generic;

namespace SpinTrack.Core.Entities.Company
{
    public class Company : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        public Guid CountryId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid TimeZoneId { get; set; }
        public Guid? DateFormatId { get; set; }

        public string? Website { get; set; }
        public string? Address { get; set; }
        public string? LogoUrl { get; set; }

        public int FiscalYearStartMonth { get; set; } = 1;

        public bool IsDeleted { get; set; }

        // Navigation properties
        public virtual Country.Country? Country { get; set; }
        public virtual Currency.Currency? Currency { get; set; }
        public virtual TimeZone.TimeZoneEntity? TimeZone { get; set; }
        public virtual DateFormat.DateFormat? DateFormat { get; set; }
        public virtual ICollection<BusinessDay.BusinessDay> BusinessDays { get; set; } = new List<BusinessDay.BusinessDay>();
        public virtual ICollection<BusinessHours.BusinessHour> BusinessHours { get; set; } = new List<BusinessHours.BusinessHour>();
        public virtual ICollection<BusinessHoliday.BusinessHoliday> BusinessHolidays { get; set; } = new List<BusinessHoliday.BusinessHoliday>();
    }
}