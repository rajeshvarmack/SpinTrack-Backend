using FluentValidation;
using Microsoft.Extensions.Logging;
using SpinTrack.Application.Common.Interfaces;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Features.Companies.DTOs;
using SpinTrack.Application.Features.Companies.Interfaces;
using SpinTrack.Application.Features.Companies.Mappers;
using SpinTrack.Core.Entities.Company;

namespace SpinTrack.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateCompanyRequest> _createValidator;
        private readonly IValidator<UpdateCompanyRequest> _updateValidator;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            ICompanyRepository companyRepository,
            ICurrentUserService currentUserService,
            IValidator<CreateCompanyRequest> createValidator,
            IValidator<UpdateCompanyRequest> updateValidator,
            ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository;
            _currentUserService = currentUserService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<Result<CompanyDetailDto>> GetCompanyByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var c = await _companyRepository.GetByIdAsync(companyId, cancellationToken);
            if (c == null)
                return Result.Failure<CompanyDetailDto>(Error.NotFound("Company", companyId.ToString()));

            return Result.Success(CompanyMapper.ToCompanyDetailDto(c));
        }

        public async Task<Result<CompanyDetailDto>> CreateCompanyAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<CompanyDetailDto>(Error.Validation("Validation failed", errors));
            }

            if (await _companyRepository.CompanyCodeExistsAsync(request.CompanyCode, cancellationToken: cancellationToken))
            {
                return Result.Failure<CompanyDetailDto>(Error.Conflict("Company code already exists"));
            }

            var c = CompanyMapper.ToEntity(request);
            await _companyRepository.AddAsync(c, cancellationToken);
            await _companyRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Company created: {CompanyId} Code: {Code}", c.CompanyId, c.CompanyCode);
            return Result.Success(CompanyMapper.ToCompanyDetailDto(c));
        }

        public async Task<Result<CompanyDetailDto>> UpdateCompanyAsync(Guid companyId, UpdateCompanyRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<CompanyDetailDto>(Error.Validation("Validation failed", errors));
            }

            var c = await _companyRepository.GetByIdAsync(companyId, cancellationToken);
            if (c == null)
                return Result.Failure<CompanyDetailDto>(Error.NotFound("Company", companyId.ToString()));

            CompanyMapper.UpdateEntity(c, request);
            _companyRepository.Update(c);
            await _companyRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Company updated: {CompanyId}", companyId);
            return Result.Success(CompanyMapper.ToCompanyDetailDto(c));
        }

        public async Task<Result> DeleteCompanyAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var c = await _companyRepository.GetByIdAsync(companyId, cancellationToken);
            if (c == null)
                return Result.Failure(Error.NotFound("Company", companyId.ToString()));

            c.IsDeleted = true;
            _companyRepository.Update(c);
            await _companyRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Company soft deleted: {CompanyId}", companyId);
            return Result.Success();
        }
    }
}
