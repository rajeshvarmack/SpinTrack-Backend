using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Users.DTOs;

namespace SpinTrack.Application.Features.Users.Interfaces
{
    /// <summary>
    /// Service for user query operations (pagination, filtering, sorting, export)
    /// </summary>
    public interface IUserQueryService
    {
        Task<Result<PagedResult<UserQueryDto>>> QueryUsersAsync(QueryRequest request, CancellationToken cancellationToken = default);
        Task<Result<ExportResult>> ExportUsersAsync(QueryRequest request, CancellationToken cancellationToken = default);
    }
}
