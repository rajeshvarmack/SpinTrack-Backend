using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.TimeZone;

namespace SpinTrack.Application.Features.TimeZones.Interfaces
{
    public interface ITimeZoneRepository
    {
        Task<TimeZoneEntity?> GetByIdAsync(Guid timeZoneId, CancellationToken cancellationToken = default);
        Task<TimeZoneEntity?> GetByNameAsync(string timeZoneName, CancellationToken cancellationToken = default);
        Task<bool> TimeZoneNameExistsAsync(string timeZoneName, Guid? excludeTimeZoneId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<TimeZoneEntity, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<TimeZoneEntity, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(TimeZoneEntity entity, CancellationToken cancellationToken = default);
        void Update(TimeZoneEntity entity);
        void Delete(TimeZoneEntity entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}