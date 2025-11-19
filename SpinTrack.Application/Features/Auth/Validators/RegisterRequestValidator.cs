using FluentValidation;
using SpinTrack.Application.Features.Auth.DTOs;

namespace SpinTrack.Application.Features.Auth.Validators
{
    /// <summary>
    /// Validator for RegisterRequest
    /// </summary>
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, hyphens and underscores");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.MiddleName)
                .MaximumLength(50).WithMessage("Middle name cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.NationalId)
                .MaximumLength(50).WithMessage("National ID cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.NationalId));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .Must(BeAValidAge).WithMessage("User must be at least 18 years old and not more than 120 years old");

            RuleFor(x => x.Nationality)
                .NotEmpty().WithMessage("Nationality is required")
                .MaximumLength(50).WithMessage("Nationality cannot exceed 50 characters");

            RuleFor(x => x.JobTitle)
                .MaximumLength(50).WithMessage("Job title cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.JobTitle));
        }

        private bool BeAValidAge(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age))
                age--;

            return age >= 18 && age <= 120;
        }
    }
}
