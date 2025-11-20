using SpinTrack.Application.Common.Models;
using SpinTrack.Core.Entities.Module;

namespace SpinTrack.Application.Features.Modules.Interfaces
{
    public interface IModuleRepository
    {
        Task<Module?> GetByIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
        Task<Module?> GetByKeyAsync(string moduleKey, CancellationToken cancellationToken = default);
        Task<bool> ModuleKeyExistsAsync(string moduleKey, Guid? excludeModuleId = null, CancellationToken cancellationToken = default);
        Task<PagedResult<TResult>> QueryAsync<TResult>(QueryRequest request, Func<Module, TResult> mapper, CancellationToken cancellationToken = default);
        Task<List<TResult>> GetAllAsync<TResult>(QueryRequest request, Func<Module, TResult> mapper, CancellationToken cancellationToken = default);

        Task AddAsync(Module module, CancellationToken cancellationToken = default);
        void Update(Module module);
        void Delete(Module module);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}