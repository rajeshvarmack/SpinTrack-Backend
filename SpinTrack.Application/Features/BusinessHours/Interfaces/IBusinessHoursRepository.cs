using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.BusinessHours;

namespace SpinTrack.Application.Features.BusinessHours.Interfaces
{
    public interface IBusinessHoursRepository
    {
        Task<BusinessHour?> GetByIdAsync(Guid businessHoursId, CancellationToken cancellationToken = default);
        Task<BusinessHour?> GetByCompanyDayShiftAsync(Guid companyId, string dayOfWeek, string shiftName, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid companyId, string dayOfWeek, string shiftName, Guid? excludeId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<BusinessHour, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<BusinessHour, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(BusinessHour entity, CancellationToken cancellationToken = default);
        void Update(BusinessHour entity);
        void Delete(BusinessHour entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}