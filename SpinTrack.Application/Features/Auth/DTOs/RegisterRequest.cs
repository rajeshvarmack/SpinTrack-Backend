using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Auth.DTOs
{
    /// <summary>
    /// Request DTO for user registration
    /// </summary>
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
    }
}
