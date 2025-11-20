using SpinTrack.Application.Features.Currencies.DTOs;
using SpinTrack.Core.Entities.Currency;

namespace SpinTrack.Application.Features.Currencies.Mappers
{
    public static class CurrencyMapper
    {
        public static CurrencyDto ToCurrencyDto(Currency c)
        {
            return new CurrencyDto
            {
                CurrencyId = c.CurrencyId,
                CurrencyCode = c.CurrencyCode,
                CurrencySymbol = c.CurrencySymbol,
                DecimalPlaces = c.DecimalPlaces,
                IsDefault = c.IsDefault,
                CreatedAt = c.CreatedAt
            };
        }

        public static CurrencyDetailDto ToCurrencyDetailDto(Currency c)
        {
            return new CurrencyDetailDto
            {
                CurrencyId = c.CurrencyId,
                CurrencyCode = c.CurrencyCode,
                CurrencySymbol = c.CurrencySymbol,
                DecimalPlaces = c.DecimalPlaces,
                IsDefault = c.IsDefault,
                CreatedAt = c.CreatedAt,
                ModifiedAt = c.ModifiedAt
            };
        }

        public static Currency ToEntity(CreateCurrencyRequest request)
        {
            return new Currency
            {
                CurrencyId = Guid.NewGuid(),
                CurrencyCode = request.CurrencyCode,
                CurrencySymbol = request.CurrencySymbol,
                DecimalPlaces = request.DecimalPlaces,
                IsDefault = request.IsDefault
            };
        }

        public static void UpdateEntity(Currency c, UpdateCurrencyRequest request)
        {
            c.CurrencySymbol = request.CurrencySymbol;
            c.DecimalPlaces = request.DecimalPlaces;
            c.IsDefault = request.IsDefault;
        }
    }
}