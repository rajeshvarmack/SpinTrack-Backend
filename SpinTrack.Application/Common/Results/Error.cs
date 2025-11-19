namespace SpinTrack.Application.Common.Results
{
    /// <summary>
    /// Represents an error with code, message, and optional details
    /// </summary>
    public class Error
    {
        public string Code { get; }
        public string Message { get; }
        public Dictionary<string, string[]>? Details { get; }

        public Error(string code, string message, Dictionary<string, string[]>? details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        // Common errors
        public static Error None => new(string.Empty, string.Empty);
        public static Error NullValue => new("ERROR.NULL_VALUE", "The specified value is null");
        public static Error NotFound(string entity, string id) => 
            new("ERROR.NOT_FOUND", $"{entity} with id '{id}' was not found");
        public static Error Validation(string message, Dictionary<string, string[]>? details = null) => 
            new("ERROR.VALIDATION", message, details);
        public static Error Conflict(string message) => 
            new("ERROR.CONFLICT", message);
        public static Error Unauthorized(string message = "Unauthorized access") => 
            new("ERROR.UNAUTHORIZED", message);
        public static Error Forbidden(string message = "Access forbidden") => 
            new("ERROR.FORBIDDEN", message);
    }
}
