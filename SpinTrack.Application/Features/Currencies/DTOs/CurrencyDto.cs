namespace SpinTrack.Application.Features.Currencies.DTOs
{
    public class CurrencyDto
    {
        public Guid CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string? CurrencySymbol { get; set; }
        public int DecimalPlaces { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}