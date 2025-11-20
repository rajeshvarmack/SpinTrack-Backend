namespace SpinTrack.Application.Features.Currencies.DTOs
{
    public class CreateCurrencyRequest
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public string? CurrencySymbol { get; set; }
        public int DecimalPlaces { get; set; } = 2;
        public bool IsDefault { get; set; } = false;
    }
}