using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.BusinessHours.DTOs;

namespace SpinTrack.Application.Features.BusinessHours.Interfaces
{
    public interface IBusinessHoursService
    {
        Task<Result<BusinessHoursDetailDto>> GetBusinessHoursByIdAsync(Guid businessHoursId, CancellationToken cancellationToken = default);
        Task<Result<BusinessHoursDetailDto>> CreateBusinessHoursAsync(CreateBusinessHoursRequest request, CancellationToken cancellationToken = default);
        Task<Result<BusinessHoursDetailDto>> UpdateBusinessHoursAsync(Guid businessHoursId, UpdateBusinessHoursRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteBusinessHoursAsync(Guid businessHoursId, CancellationToken cancellationToken = default);
    }
}