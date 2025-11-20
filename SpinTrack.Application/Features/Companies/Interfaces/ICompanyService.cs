using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Companies.DTOs;

namespace SpinTrack.Application.Features.Companies.Interfaces
{
    public interface ICompanyService
    {
        Task<Result<CompanyDetailDto>> GetCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<Result<CompanyDetailDto>> CreateCompanyAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default);
        Task<Result<CompanyDetailDto>> UpdateCompanyAsync(Guid companyId, UpdateCompanyRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteCompanyAsync(Guid companyId, CancellationToken cancellationToken = default);
    }
}