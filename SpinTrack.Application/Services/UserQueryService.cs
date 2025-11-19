using FluentValidation;
using SpinTrack.Application.Common.Models;
using SpinTrack.Application.Common.Results;
using SpinTrack.Application.Common.Services;
using SpinTrack.Application.Features.Auth.Mappers;
using SpinTrack.Application.Features.Users.DTOs;
using SpinTrack.Application.Features.Users.Interfaces;

namespace SpinTrack.Application.Services
{
    /// <summary>
    /// User query service implementation for querying and exporting users
    /// </summary>
    public class UserQueryService : IUserQueryService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICsvExportService _csvExportService;
        private readonly IExcelExportService _excelExportService;
        private readonly IValidator<QueryRequest> _queryRequestValidator;

        public UserQueryService(
            IUserRepository userRepository,
            ICsvExportService csvExportService,
            IExcelExportService excelExportService,
            IValidator<QueryRequest> queryRequestValidator)
        {
            _userRepository = userRepository;
            _csvExportService = csvExportService;
            _excelExportService = excelExportService;
            _queryRequestValidator = queryRequestValidator;
        }

        public async Task<Result<PagedResult<UserQueryDto>>> QueryUsersAsync(QueryRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request
            var validationResult = await _queryRequestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<PagedResult<UserQueryDto>>(Error.Validation("Validation failed", errors));
            }

            var result = await _userRepository.QueryAsync(
                request,
                UserMapper.ToUserQueryDto,
                cancellationToken);

            return Result.Success(result);
        }

        public async Task<Result<ExportResult>> ExportUsersAsync(QueryRequest request, CancellationToken cancellationToken = default)
        {
            // Validate request
            var validationResult = await _queryRequestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result.Failure<ExportResult>(Error.Validation("Validation failed", errors));
            }

            // Get all users based on filters
            var users = await _userRepository.GetAllAsync(
                request,
                UserMapper.ToUserQueryDto,
                cancellationToken);

            // Define column mappings for export
            var columnMappings = new Dictionary<string, Func<UserQueryDto, object>>
            {
                { "User ID", u => u.UserId },
                { "Username", u => u.Username },
                { "Email", u => u.Email },
                { "First Name", u => u.FirstName },
                { "Middle Name", u => u.MiddleName ?? string.Empty },
                { "Last Name", u => u.LastName ?? string.Empty },
                { "Full Name", u => u.FullName },
                { "Phone Number", u => u.PhoneNumber ?? string.Empty },
                { "Gender", u => u.Gender },
                { "Date of Birth", u => u.DateOfBirth.ToString("yyyy-MM-dd") },
                { "Age", u => u.Age },
                { "Nationality", u => u.Nationality },
                { "Job Title", u => u.JobTitle ?? string.Empty },
                { "Status", u => u.Status },
                { "Created At", u => u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") }
            };

            // Export based on format
            byte[] fileContent;
            string fileName;
            string contentType;

            if (request.ExportFormat == ExportFormat.Excel)
            {
                // Export to Excel
                fileContent = _excelExportService.ExportToExcel(users, columnMappings);
                fileName = $"Users_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else
            {
                // Export to CSV (default)
                fileContent = _csvExportService.ExportToCsv(users, columnMappings);
                fileName = $"Users_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
                contentType = "text/csv";
            }

            var exportResult = new ExportResult
            {
                FileContent = fileContent,
                FileName = fileName,
                ContentType = contentType
            };

            return Result.Success(exportResult);
        }
    }
}
