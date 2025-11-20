using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.DateFormats.DTOs;

namespace SpinTrack.Application.Features.DateFormats.Interfaces
{
    public interface IDateFormatService
    {
        Task<Result<DateFormatDetailDto>> GetDateFormatByIdAsync(Guid dateFormatId, CancellationToken cancellationToken = default);
        Task<Result<DateFormatDetailDto>> CreateDateFormatAsync(CreateDateFormatRequest request, CancellationToken cancellationToken = default);
        Task<Result<DateFormatDetailDto>> UpdateDateFormatAsync(Guid dateFormatId, UpdateDateFormatRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteDateFormatAsync(Guid dateFormatId, CancellationToken cancellationToken = default);
    }
}