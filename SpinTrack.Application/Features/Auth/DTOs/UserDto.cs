using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Auth.DTOs
{
    /// <summary>
    /// DTO for user information in auth responses
    /// </summary>
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public UserStatus Status { get; set; }
    }
}
