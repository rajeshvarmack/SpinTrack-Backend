using SpinTrack.Application.Features.Auth.DTOs;
using SpinTrack.Application.Features.Users.DTOs;
using SpinTrack.Core.Entities.Auth;

namespace SpinTrack.Application.Features.Auth.Mappers
{
    /// <summary>
    /// Manual mapper for User entity to DTOs
    /// </summary>
    public static class UserMapper
    {
        public static UserDto ToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                FullName = user.GetFullName(),
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Status = user.Status
            };
        }

        public static UserDetailDto ToUserDetailDto(User user)
        {
            return new UserDetailDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                FullName = user.GetFullName(),
                PhoneNumber = user.PhoneNumber,
                NationalId = user.NationalId,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = user.GetAge(),
                Nationality = user.Nationality,
                JobTitle = user.JobTitle,
                ProfilePicturePath = user.ProfilePicturePath,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt
            };
        }

        public static UserQueryDto ToUserQueryDto(User user)
        {
            return new UserQueryDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                FullName = user.GetFullName(),
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = user.GetAge(),
                Nationality = user.Nationality,
                JobTitle = user.JobTitle,
                Status = user.Status,
                CreatedAt = user.CreatedAt
            };
        }

        public static User ToEntity(RegisterRequest request, string passwordHash)
        {
            return new User
            {
                UserId = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                NationalId = request.NationalId,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                Nationality = request.Nationality,
                JobTitle = request.JobTitle,
                Status = Core.Enums.UserStatus.Active
            };
        }

        public static void UpdateEntity(User user, UpdateUserRequest request)
        {
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.MiddleName = request.MiddleName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.NationalId = request.NationalId;
            user.Gender = request.Gender;
            user.DateOfBirth = request.DateOfBirth;
            user.Nationality = request.Nationality;
            user.JobTitle = request.JobTitle;
        }
    }
}
