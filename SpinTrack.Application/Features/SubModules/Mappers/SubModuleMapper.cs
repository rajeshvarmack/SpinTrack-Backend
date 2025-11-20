using SpinTrack.Application.Features.SubModules.DTOs;
using SpinTrack.Core.Entities.SubModule;

namespace SpinTrack.Application.Features.SubModules.Mappers
{
    public static class SubModuleMapper
    {
        public static SubModuleDto ToSubModuleDto(SubModule sub)
        {
            return new SubModuleDto
            {
                SubModuleId = sub.SubModuleId,
                ModuleId = sub.ModuleId,
                SubModuleKey = sub.SubModuleKey,
                SubModuleName = sub.SubModuleName,
                Status = sub.Status,
                CreatedAt = sub.CreatedAt
            };
        }

        public static SubModuleDetailDto ToSubModuleDetailDto(SubModule sub)
        {
            return new SubModuleDetailDto
            {
                SubModuleId = sub.SubModuleId,
                ModuleId = sub.ModuleId,
                SubModuleKey = sub.SubModuleKey,
                SubModuleName = sub.SubModuleName,
                Status = sub.Status,
                CreatedAt = sub.CreatedAt,
                ModifiedAt = sub.ModifiedAt
            };
        }

        public static SubModule ToEntity(CreateSubModuleRequest request)
        {
            return new SubModule
            {
                SubModuleId = Guid.NewGuid(),
                ModuleId = request.ModuleId,
                SubModuleKey = request.SubModuleKey,
                SubModuleName = request.SubModuleName,
                Status = Core.Enums.ModuleStatus.Active
            };
        }

        public static void UpdateEntity(SubModule sub, UpdateSubModuleRequest request)
        {
            sub.ModuleId = request.ModuleId;
            sub.SubModuleName = request.SubModuleName;
        }
    }
}