using SpinTrack.Core.Enums;

namespace SpinTrack.Application.Features.Users.DTOs
{
    /// <summary>
    /// DTO for detailed user information
    /// </summary>
    public class UserDetailDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? ProfilePicturePath { get; set; }
        public UserStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
