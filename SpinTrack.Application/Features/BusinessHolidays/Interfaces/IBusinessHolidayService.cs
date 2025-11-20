using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.BusinessHolidays.DTOs;

namespace SpinTrack.Application.Features.BusinessHolidays.Interfaces
{
    public interface IBusinessHolidayService
    {
        Task<Result<BusinessHolidayDetailDto>> GetBusinessHolidayByIdAsync(Guid businessHolidayId, CancellationToken cancellationToken = default);
        Task<Result<BusinessHolidayDetailDto>> CreateBusinessHolidayAsync(CreateBusinessHolidayRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusinessHolidayDetailDto>> UpdateBusinessHolidayAsync(Guid businessHolidayId, UpdateBusinessHolidayRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteBusinessHolidayAsync(Guid businessHolidayId, CancellationToken cancellationToken = default);
    }
}