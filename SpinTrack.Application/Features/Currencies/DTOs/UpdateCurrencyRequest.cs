namespace SpinTrack.Application.Features.Currencies.DTOs
{
    public class UpdateCurrencyRequest
    {
        public string? CurrencySymbol { get; set; }
        public int DecimalPlaces { get; set; } = 2;
        public bool IsDefault { get; set; } = false;
    }
}