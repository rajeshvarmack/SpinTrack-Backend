using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Modules.DTOs;

namespace SpinTrack.Application.Features.Modules.Interfaces
{
    public interface IModuleService
    {
        Task<Result<ModuleDetailDto>> GetModuleByIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
        Task<Result<ModuleDetailDto>> CreateModuleAsync(CreateModuleRequest request, CancellationToken cancellationToken = default);
        Task<Result<ModuleDetailDto>> UpdateModuleAsync(Guid moduleId, UpdateModuleRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default);
        Task<Result> ChangeModuleStatusAsync(Guid moduleId, ChangeModuleStatusRequest request, CancellationToken cancellationToken = default);
    }
}