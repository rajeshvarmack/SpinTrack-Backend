using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.TimeZones.DTOs;

namespace SpinTrack.Application.Features.TimeZones.Interfaces
{
    public interface ITimeZoneService
    {
        Task<Result<TimeZoneDetailDto>> GetTimeZoneByIdAsync(Guid timeZoneId, CancellationToken cancellationToken = default);
        Task<Result<TimeZoneDetailDto>> CreateTimeZoneAsync(CreateTimeZoneRequest request, CancellationToken cancellationToken = default);
        Task<Result<TimeZoneDetailDto>> UpdateTimeZoneAsync(Guid timeZoneId, UpdateTimeZoneRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteTimeZoneAsync(Guid timeZoneId, CancellationToken cancellationToken = default);
    }
}