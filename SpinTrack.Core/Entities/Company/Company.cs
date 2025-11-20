using SpinTrack.Core.Entities.Common;

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
    }
}