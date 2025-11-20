using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.BusinessDay;

namespace SpinTrack.Application.Features.BusinessDays.Interfaces
{
    public interface IBusinessDayRepository
    {
        Task<BusinessDay?> GetByIdAsync(Guid businessDayId, CancellationToken cancellationToken = default);
        Task<BusinessDay?> GetByCompanyAndDayAsync(Guid companyId, string dayOfWeek, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid companyId, string dayOfWeek, Guid? excludeId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<BusinessDay, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<BusinessDay, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(BusinessDay entity, CancellationToken cancellationToken = default);
        void Update(BusinessDay entity);
        void Delete(BusinessDay entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}