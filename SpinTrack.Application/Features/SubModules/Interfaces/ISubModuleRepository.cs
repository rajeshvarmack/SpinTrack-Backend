using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.SubModule;

namespace SpinTrack.Application.Features.SubModules.Interfaces
{
    public interface ISubModuleRepository
    {
        Task<SubModule?> GetByIdAsync(Guid subModuleId, CancellationToken cancellationToken = default);
        Task<SubModule?> GetByKeyAsync(string subModuleKey, CancellationToken cancellationToken = default);
        Task<bool> SubModuleKeyExistsAsync(string subModuleKey, Guid? excludeSubModuleId = null, CancellationToken cancellationToken = default);
        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<SubModule, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<SubModule, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(SubModule subModule, CancellationToken cancellationToken = default);
        void Update(SubModule subModule);
        void Delete(SubModule subModule);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}