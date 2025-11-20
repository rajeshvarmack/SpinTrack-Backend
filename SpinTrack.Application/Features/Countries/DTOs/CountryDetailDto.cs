namespace SpinTrack.Application.Features.Countries.DTOs
{
    public class CountryDetailDto
    {
        public Guid CountryId { get; set; }
        public string CountryCodeISO2 { get; set; } = string.Empty;
        public string CountryCodeISO3 { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string? PhoneCode { get; set; }
        public string? Continent { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}