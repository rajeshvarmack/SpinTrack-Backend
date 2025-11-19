using FluentValidation;

namespace SpinTrack.Application.Common.Behaviors
{
    /// <summary>
    /// Validation behavior for automatic validation before service execution
    /// </summary>
    public class ValidationBehavior
    {
        public static async Task ValidateAsync<T>(T request, IValidator<T> validator)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
