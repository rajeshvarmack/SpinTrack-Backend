namespace SpinTrack.Application.Common.Services
{
    /// <summary>
    /// Service for file storage operations (local or cloud)
    /// </summary>
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task<Stream?> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default);
        Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);
        string GetFileUrl(string filePath);
    }
}
