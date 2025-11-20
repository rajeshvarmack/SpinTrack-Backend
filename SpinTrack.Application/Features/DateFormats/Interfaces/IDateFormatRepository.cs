using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.DateFormat;

namespace SpinTrack.Application.Features.DateFormats.Interfaces
{
    public interface IDateFormatRepository
    {
        Task<DateFormat?> GetByIdAsync(Guid dateFormatId, CancellationToken cancellationToken = default);
        Task<DateFormat?> GetByFormatAsync(string formatString, CancellationToken cancellationToken = default);
        Task<bool> FormatExistsAsync(string formatString, Guid? excludeDateFormatId = null, CancellationToken cancellationToken = default);

        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<DateFormat, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<DateFormat, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(DateFormat entity, CancellationToken cancellationToken = default);
        void Update(DateFormat entity);
        void Delete(DateFormat entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}