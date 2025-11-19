using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Users.DTOs;

namespace SpinTrack.Application.Features.Users.Interfaces
{
    /// <summary>
    /// Service for user profile management
    /// </summary>
    public interface IUserProfileService
    {
        Task<Result<UserDetailDto>> GetCurrentUserProfileAsync(CancellationToken cancellationToken = default);
        Task<Result<UserDetailDto>> UpdateCurrentUserProfileAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result<UserDetailDto>> UploadProfilePictureAsync(FileUpload file, CancellationToken cancellationToken = default);
        Task<Result> DeleteProfilePictureAsync(CancellationToken cancellationToken = default);
    }
}
