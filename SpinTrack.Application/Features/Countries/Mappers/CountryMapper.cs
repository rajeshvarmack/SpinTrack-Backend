using SpinTrack.Application.Features.Countries.DTOs;
using SpinTrack.Core.Entities.Country;

namespace SpinTrack.Application.Features.Countries.Mappers
{
    public static class CountryMapper
    {
        public static CountryDto ToCountryDto(Country c)
        {
            return new CountryDto
            {
                CountryId = c.CountryId,
                CountryCodeISO2 = c.CountryCodeISO2,
                CountryCodeISO3 = c.CountryCodeISO3,
                CountryName = c.CountryName,
                PhoneCode = c.PhoneCode,
                Continent = c.Continent,
                CreatedAt = c.CreatedAt
            };
        }

        public static CountryDetailDto ToCountryDetailDto(Country c)
        {
            return new CountryDetailDto
            {
                CountryId = c.CountryId,
                CountryCodeISO2 = c.CountryCodeISO2,
                CountryCodeISO3 = c.CountryCodeISO3,
                CountryName = c.CountryName,
                PhoneCode = c.PhoneCode,
                Continent = c.Continent,
                CreatedAt = c.CreatedAt,
                ModifiedAt = c.ModifiedAt
            };
        }

        public static Country ToEntity(CreateCountryRequest request)
        {
            return new Country
            {
                CountryId = Guid.NewGuid(),
                CountryCodeISO2 = request.CountryCodeISO2,
                CountryCodeISO3 = request.CountryCodeISO3,
                CountryName = request.CountryName,
                PhoneCode = request.PhoneCode,
                Continent = request.Continent
            };
        }

        public static void UpdateEntity(Country country, UpdateCountryRequest request)
        {
            country.CountryCodeISO3 = request.CountryCodeISO3;
            country.CountryName = request.CountryName;
            country.PhoneCode = request.PhoneCode;
            country.Continent = request.Continent;
        }
    }
}