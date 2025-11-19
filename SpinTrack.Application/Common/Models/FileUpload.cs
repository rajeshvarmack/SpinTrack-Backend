namespace SpinTrack.Application.Common.Models
{
    /// <summary>
    /// Represents a file upload abstraction (Clean Architecture - no ASP.NET Core dependency)
    /// </summary>
    public class FileUpload
    {
        public Stream Content { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Length { get; set; }
    }
}
