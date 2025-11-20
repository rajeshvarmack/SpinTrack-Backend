using SpinTrack.Application.Features.BusinessHolidays.DTOs;
using SpinTrack.Core.Entities.BusinessHoliday;

namespace SpinTrack.Application.Features.BusinessHolidays.Mappers
{
    public static class BusinessHolidayMapper
    {
        public static BusinessHolidayDto ToBusinessHolidayDto(BusinessHoliday bh)
        {
            return new BusinessHolidayDto
            {
                BusinessHolidayId = bh.BusinessHolidayId,
                CompanyId = bh.CompanyId,
                HolidayDate = bh.HolidayDate,
                HolidayName = bh.HolidayName,
                HolidayType = bh.HolidayType,
                CountryId = bh.CountryId,
                IsFullDay = bh.IsFullDay,
                StartTime = bh.StartTime,
                EndTime = bh.EndTime,
                CreatedAt = bh.CreatedAt
            };
        }

        public static BusinessHolidayDetailDto ToBusinessHolidayDetailDto(BusinessHoliday bh)
        {
            return new BusinessHolidayDetailDto
            {
                BusinessHolidayId = bh.BusinessHolidayId,
                CompanyId = bh.CompanyId,
                HolidayDate = bh.HolidayDate,
                HolidayName = bh.HolidayName,
                HolidayType = bh.HolidayType,
                CountryId = bh.CountryId,
                IsFullDay = bh.IsFullDay,
                StartTime = bh.StartTime,
                EndTime = bh.EndTime,
                CreatedAt = bh.CreatedAt,
                ModifiedAt = bh.ModifiedAt
            };
        }

        public static BusinessHoliday ToEntity(CreateBusinessHolidayRequest request)
        {
            return new BusinessHoliday
            {
                BusinessHolidayId = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                HolidayDate = request.HolidayDate,
                HolidayName = request.HolidayName,
                HolidayType = request.HolidayType,
                CountryId = request.CountryId,
                IsFullDay = request.IsFullDay,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };
        }

        public static void UpdateEntity(BusinessHoliday bh, UpdateBusinessHolidayRequest request)
        {
            bh.HolidayName = request.HolidayName;
            bh.HolidayType = request.HolidayType;
            bh.CountryId = request.CountryId;
            bh.IsFullDay = request.IsFullDay;
            bh.StartTime = request.StartTime;
            bh.EndTime = request.EndTime;
        }
    }
}