using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.BusinessDays.DTOs;

namespace SpinTrack.Application.Features.BusinessDays.Interfaces
{
    public interface IBusinessDayService
    {
        Task<Result<BusinessDayDetailDto>> GetBusinessDayByIdAsync(Guid businessDayId, CancellationToken cancellationToken = default);
        Task<Result<BusinessDayDetailDto>> CreateBusinessDayAsync(CreateBusinessDayRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusinessDayDetailDto>> UpdateBusinessDayAsync(Guid businessDayId, UpdateBusinessDayRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteBusinessDayAsync(Guid businessDayId, CancellationToken cancellationToken = default);
    }
}