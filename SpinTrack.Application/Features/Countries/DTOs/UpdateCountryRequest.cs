namespace SpinTrack.Application.Features.Countries.DTOs
{
    public class UpdateCountryRequest
    {
        public string CountryCodeISO3 { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string? PhoneCode { get; set; }
        public string? Continent { get; set; }
    }
}