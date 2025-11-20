using SpinTrack.Application.Features.Permissions.DTOs;
using SpinTrack.Core.Entities.Permission;

namespace SpinTrack.Application.Features.Permissions.Mappers
{
    public static class PermissionMapper
    {
        public static PermissionDto ToPermissionDto(Permission p)
        {
            return new PermissionDto
            {
                PermissionId = p.PermissionId,
                SubModuleId = p.SubModuleId,
                PermissionKey = p.PermissionKey,
                PermissionName = p.PermissionName,
                Status = p.Status,
                CreatedAt = p.CreatedAt
            };
        }

        public static PermissionDetailDto ToPermissionDetailDto(Permission p)
        {
            return new PermissionDetailDto
            {
                PermissionId = p.PermissionId,
                SubModuleId = p.SubModuleId,
                PermissionKey = p.PermissionKey,
                PermissionName = p.PermissionName,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                ModifiedAt = p.ModifiedAt
            };
        }

        public static Permission ToEntity(CreatePermissionRequest request)
        {
            return new Permission
            {
                PermissionId = Guid.NewGuid(),
                SubModuleId = request.SubModuleId,
                PermissionKey = request.PermissionKey,
                PermissionName = request.PermissionName,
                Status = Core.Enums.ModuleStatus.Active
            };
        }

        public static void UpdateEntity(Permission p, UpdatePermissionRequest request)
        {
            p.SubModuleId = request.SubModuleId;
            p.PermissionName = request.PermissionName;
        }
    }
}