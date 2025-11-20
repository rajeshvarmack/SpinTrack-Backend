using SpinTrack.Application.Features.BusinessDays.DTOs;
using SpinTrack.Core.Entities.BusinessDay;

namespace SpinTrack.Application.Features.BusinessDays.Mappers
{
    public static class BusinessDayMapper
    {
        public static BusinessDayDto ToBusinessDayDto(BusinessDay bd)
        {
            return new BusinessDayDto
            {
                BusinessDayId = bd.BusinessDayId,
                CompanyId = bd.CompanyId,
                DayOfWeek = bd.DayOfWeek,
                IsWorkingDay = bd.IsWorkingDay,
                IsWeekend = bd.IsWeekend,
                Remarks = bd.Remarks,
                CreatedAt = bd.CreatedAt
            };
        }

        public static BusinessDayDetailDto ToBusinessDayDetailDto(BusinessDay bd)
        {
            return new BusinessDayDetailDto
            {
                BusinessDayId = bd.BusinessDayId,
                CompanyId = bd.CompanyId,
                DayOfWeek = bd.DayOfWeek,
                IsWorkingDay = bd.IsWorkingDay,
                IsWeekend = bd.IsWeekend,
                Remarks = bd.Remarks,
                CreatedAt = bd.CreatedAt,
                ModifiedAt = bd.ModifiedAt
            };
        }

        public static BusinessDay ToEntity(CreateBusinessDayRequest request)
        {
            return new BusinessDay
            {
                BusinessDayId = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                DayOfWeek = request.DayOfWeek,
                IsWorkingDay = request.IsWorkingDay,
                IsWeekend = request.IsWeekend,
                Remarks = request.Remarks
            };
        }

        public static void UpdateEntity(BusinessDay bd, UpdateBusinessDayRequest request)
        {
            bd.IsWorkingDay = request.IsWorkingDay;
            bd.IsWeekend = request.IsWeekend;
            bd.Remarks = request.Remarks;
        }
    }
}