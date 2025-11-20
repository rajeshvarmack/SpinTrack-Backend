using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.BusinessHoliday;

namespace SpinTrack.Application.Features.BusinessHolidays.Interfaces
{
    public interface IBusinessHolidayRepository
    {
        Task<BusinessHoliday?> GetByIdAsync(Guid businessHolidayId, CancellationToken cancellationToken = default);
        Task<BusinessHoliday?> GetByCompanyAndDateAsync(Guid companyId, DateOnly holidayDate, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid companyId, DateOnly holidayDate, Guid? excludeId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<BusinessHoliday, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<BusinessHoliday, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(BusinessHoliday entity, CancellationToken cancellationToken = default);
        void Update(BusinessHoliday entity);
        void Delete(BusinessHoliday entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}