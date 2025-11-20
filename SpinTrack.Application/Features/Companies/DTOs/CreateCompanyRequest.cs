namespace SpinTrack.Application.Features.Companies.DTOs
{
    public class CreateCompanyRequest
    {
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
    }
}