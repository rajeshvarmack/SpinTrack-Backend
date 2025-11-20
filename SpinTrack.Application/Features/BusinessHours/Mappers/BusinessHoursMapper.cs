using SpinTrack.Application.Features.BusinessHours.DTOs;
using SpinTrack.Core.Entities.BusinessHours;

namespace SpinTrack.Application.Features.BusinessHours.Mappers
{
    public static class BusinessHoursMapper
    {
        public static BusinessHoursDto ToBusinessHoursDto(BusinessHour bh)
        {
            return new BusinessHoursDto
            {
                BusinessHoursId = bh.BusinessHoursId,
                CompanyId = bh.CompanyId,
                DayOfWeek = bh.DayOfWeek,
                ShiftName = bh.ShiftName,
                StartTime = bh.StartTime,
                EndTime = bh.EndTime,
                IsWorkingShift = bh.IsWorkingShift,
                IsOvertimeEligible = bh.IsOvertimeEligible,
                Remarks = bh.Remarks,
                CreatedAt = bh.CreatedAt
            };
        }

        public static BusinessHoursDetailDto ToBusinessHoursDetailDto(BusinessHour bh)
        {
            return new BusinessHoursDetailDto
            {
                BusinessHoursId = bh.BusinessHoursId,
                CompanyId = bh.CompanyId,
                DayOfWeek = bh.DayOfWeek,
                ShiftName = bh.ShiftName,
                StartTime = bh.StartTime,
                EndTime = bh.EndTime,
                IsWorkingShift = bh.IsWorkingShift,
                IsOvertimeEligible = bh.IsOvertimeEligible,
                Remarks = bh.Remarks,
                CreatedAt = bh.CreatedAt,
                ModifiedAt = bh.ModifiedAt
            };
        }

        public static BusinessHour ToEntity(CreateBusinessHoursRequest request)
        {
            return new BusinessHour
            {
                BusinessHoursId = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                DayOfWeek = request.DayOfWeek,
                ShiftName = request.ShiftName,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsWorkingShift = request.IsWorkingShift,
                IsOvertimeEligible = request.IsOvertimeEligible,
                Remarks = request.Remarks
            };
        }

        public static void UpdateEntity(BusinessHour bh, UpdateBusinessHoursRequest request)
        {
            bh.StartTime = request.StartTime;
            bh.EndTime = request.EndTime;
            bh.IsWorkingShift = request.IsWorkingShift;
            bh.IsOvertimeEligible = request.IsOvertimeEligible;
            bh.Remarks = request.Remarks;
        }
    }
}