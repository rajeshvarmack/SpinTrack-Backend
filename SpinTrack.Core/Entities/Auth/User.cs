using SpinTrack.Core.Entities.Common;
using SpinTrack.Core.Enums;

namespace SpinTrack.Core.Entities.Auth
{
    /// <summary>
    /// User entity representing application users
    /// </summary>
    public class User : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string? ProfilePicturePath { get; set; }
        public UserStatus Status { get; set; }

        // Account Lockout Properties
        public int FailedLoginAttempts { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        // Soft Delete Properties
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation properties
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // Helper methods
        public string GetFullName()
        {
            var parts = new List<string> { FirstName };
            if (!string.IsNullOrWhiteSpace(MiddleName))
                parts.Add(MiddleName);
            if (!string.IsNullOrWhiteSpace(LastName))
                parts.Add(LastName);
            return string.Join(" ", parts);
        }

        public int GetAge()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth > today.AddYears(-age))
                age--;
            return age;
        }

        public bool IsLockedOut()
        {
            return LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
        }

        public void ResetFailedLoginAttempts()
        {
            FailedLoginAttempts = 0;
            LockoutEnd = null;
        }

        public void IncrementFailedLoginAttempts(int maxAttempts = 5, int lockoutMinutes = 30)
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= maxAttempts)
            {
                LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(lockoutMinutes);
            }
        }
    }
}
