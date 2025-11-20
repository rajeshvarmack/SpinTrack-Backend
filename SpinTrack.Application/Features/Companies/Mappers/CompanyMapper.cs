using SpinTrack.Application.Features.Companies.DTOs;
using SpinTrack.Core.Entities.Company;

namespace SpinTrack.Application.Features.Companies.Mappers
{
    public static class CompanyMapper
    {
        public static CompanyDto ToCompanyDto(Company c)
        {
            return new CompanyDto
            {
                CompanyId = c.CompanyId,
                CompanyCode = c.CompanyCode,
                CompanyName = c.CompanyName,
                CountryId = c.CountryId,
                CurrencyId = c.CurrencyId,
                TimeZoneId = c.TimeZoneId,
                DateFormatId = c.DateFormatId,
                Website = c.Website,
                Address = c.Address,
                LogoUrl = c.LogoUrl,
                FiscalYearStartMonth = c.FiscalYearStartMonth,
                CreatedAt = c.CreatedAt
            };
        }

        public static CompanyDetailDto ToCompanyDetailDto(Company c)
        {
            return new CompanyDetailDto
            {
                CompanyId = c.CompanyId,
                CompanyCode = c.CompanyCode,
                CompanyName = c.CompanyName,
                CountryId = c.CountryId,
                CurrencyId = c.CurrencyId,
                TimeZoneId = c.TimeZoneId,
                DateFormatId = c.DateFormatId,
                Website = c.Website,
                Address = c.Address,
                LogoUrl = c.LogoUrl,
                FiscalYearStartMonth = c.FiscalYearStartMonth,
                CreatedAt = c.CreatedAt,
                ModifiedAt = c.ModifiedAt
            };
        }

        public static Company ToEntity(CreateCompanyRequest request)
        {
            return new Company
            {
                CompanyId = Guid.NewGuid(),
                CompanyCode = request.CompanyCode,
                CompanyName = request.CompanyName,
                CountryId = request.CountryId,
                CurrencyId = request.CurrencyId,
                TimeZoneId = request.TimeZoneId,
                DateFormatId = request.DateFormatId,
                Website = request.Website,
                Address = request.Address,
                LogoUrl = request.LogoUrl,
                FiscalYearStartMonth = request.FiscalYearStartMonth
            };
        }

        public static void UpdateEntity(Company c, UpdateCompanyRequest request)
        {
            c.CompanyName = request.CompanyName;
            c.CountryId = request.CountryId;
            c.CurrencyId = request.CurrencyId;
            c.TimeZoneId = request.TimeZoneId;
            c.DateFormatId = request.DateFormatId;
            c.Website = request.Website;
            c.Address = request.Address;
            c.LogoUrl = request.LogoUrl;
            c.FiscalYearStartMonth = request.FiscalYearStartMonth;
        }
    }
}