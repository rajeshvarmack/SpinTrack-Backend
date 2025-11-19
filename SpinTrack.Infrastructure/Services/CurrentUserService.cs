using Microsoft.AspNetCore.Http;
using SpinTrack.Application.Common.Interfaces;
using System.Security.Claims;

namespace SpinTrack.Infrastructure.Services
{
    /// <summary>
    /// Service to get current authenticated user information from HTTP context
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;

        public string? MiddleName => _httpContextAccessor.HttpContext?.User?.FindFirst("MiddleName")?.Value;

        public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value;

        public string? PhoneNumber => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.MobilePhone)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
