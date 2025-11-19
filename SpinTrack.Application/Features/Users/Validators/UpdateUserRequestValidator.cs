using FluentValidation;
using SpinTrack.Application.Features.Users.DTOs;

namespace SpinTrack.Application.Features.Users.Validators
{
    /// <summary>
    /// Validator for UpdateUserRequest
    /// </summary>
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

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
