namespace SpinTrack.Application.Common.Models
{
    /// <summary>
    /// Result of an export operation
    /// </summary>
    public class ExportResult
    {
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
