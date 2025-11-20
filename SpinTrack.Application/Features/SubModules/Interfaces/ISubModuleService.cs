using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.SubModules.DTOs;

namespace SpinTrack.Application.Features.SubModules.Interfaces
{
    public interface ISubModuleService
    {
        Task<Result<SubModuleDetailDto>> GetSubModuleByIdAsync(Guid subModuleId, CancellationToken cancellationToken = default);
        Task<Result<SubModuleDetailDto>> CreateSubModuleAsync(CreateSubModuleRequest request, CancellationToken cancellationToken = default);
        Task<Result<SubModuleDetailDto>> UpdateSubModuleAsync(Guid subModuleId, UpdateSubModuleRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteSubModuleAsync(Guid subModuleId, CancellationToken cancellationToken = default);
        Task<Result> ChangeSubModuleStatusAsync(Guid subModuleId, ChangeSubModuleStatusRequest request, CancellationToken cancellationToken = default);
    }
}