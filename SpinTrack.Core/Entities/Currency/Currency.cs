using SpinTrack.Core.Entities.Common;

namespace SpinTrack.Core.Entities.Currency
{
    public class Currency : BaseEntity
    {
        public Guid CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string? CurrencySymbol { get; set; }
        public int DecimalPlaces { get; set; } = 2;
        public bool IsDefault { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
    }
}