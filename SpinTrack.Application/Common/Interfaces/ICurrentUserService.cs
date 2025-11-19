namespace SpinTrack.Application.Common.Interfaces
{
    /// <summary>
    /// Service to get current authenticated user information
    /// </summary>
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Username { get; }
        string? Email { get; }
        string? FirstName { get; }
        string? MiddleName { get; }
        string? LastName { get; }
        string? PhoneNumber { get; }
        bool IsAuthenticated { get; }
    }
}
