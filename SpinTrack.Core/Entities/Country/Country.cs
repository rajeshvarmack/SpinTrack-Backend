using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.Country
{
    public class Country : BaseEntity
    {
        public Guid CountryId { get; set; }
        public string CountryCodeISO2 { get; set; } = string.Empty;
        public string CountryCodeISO3 { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string? PhoneCode { get; set; }
        public string? Continent { get; set; }

        public bool IsDeleted { get; set; }
    }
}