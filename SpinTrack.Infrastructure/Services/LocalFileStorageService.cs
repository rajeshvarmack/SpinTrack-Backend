using Microsoft.Extensions.Options;
using SpinTrack.Application.Common.Services;
using SpinTrack.Application.Common.Settings;

namespace SpinTrack.Infrastructure.Services
{
    /// <summary>
    /// Local file storage service implementation with validation
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly FileStorageSettings _settings;
        private readonly string _basePath;

        public LocalFileStorageService(IOptions<FileStorageSettings> settings)
        {
            _settings = settings.Value;
            _basePath = Path.Combine(Directory.GetCurrentDirectory(), _settings.LocalBasePath);

            // Ensure directory exists
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            // Validate file size
            if (fileStream.Length > _settings.MaxFileSizeInMB * 1024 * 1024)
            {
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {_settings.MaxFileSizeInMB}MB");
            }

            // Validate file extension
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", _settings.AllowedExtensions)}");
            }

            // Sanitize and generate unique filename
            var sanitizedFileName = SanitizeFileName(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(sanitizedFileName)}";
            var filePath = Path.Combine(_basePath, uniqueFileName);

            // Ensure the path is within the base directory (prevent path traversal)
            var normalizedPath = Path.GetFullPath(filePath);
            var normalizedBase = Path.GetFullPath(_basePath);
            
            if (!normalizedPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid file path detected");
            }

            // Save file to disk
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
            }

            // Return relative path
            return Path.Combine(_settings.LocalBasePath, uniqueFileName).Replace("\\", "/");
        }

        public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                // Extract only the filename to prevent path traversal
                var fileName = Path.GetFileName(filePath);
                var fullPath = Path.Combine(_basePath, fileName);

                // Ensure the path is within the base directory
                var normalizedPath = Path.GetFullPath(fullPath);
                var normalizedBase = Path.GetFullPath(_basePath);

                if (!normalizedPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(false); // Path traversal attempt blocked
                }

                if (File.Exists(normalizedPath))
                {
                    File.Delete(normalizedPath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<Stream?> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                // Extract only the filename to prevent path traversal
                var fileName = Path.GetFileName(filePath);
                var fullPath = Path.Combine(_basePath, fileName);

                // Ensure the path is within the base directory
                var normalizedPath = Path.GetFullPath(fullPath);
                var normalizedBase = Path.GetFullPath(_basePath);

                if (!normalizedPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult<Stream?>(null); // Path traversal attempt blocked
                }

                if (File.Exists(normalizedPath))
                {
                    var stream = new FileStream(normalizedPath, FileMode.Open, FileAccess.Read);
                    return Task.FromResult<Stream?>(stream);
                }

                return Task.FromResult<Stream?>(null);
            }
            catch
            {
                return Task.FromResult<Stream?>(null);
            }
        }

        public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                // Extract only the filename to prevent path traversal
                var fileName = Path.GetFileName(filePath);
                var fullPath = Path.Combine(_basePath, fileName);

                // Ensure the path is within the base directory
                var normalizedPath = Path.GetFullPath(fullPath);
                var normalizedBase = Path.GetFullPath(_basePath);

                if (!normalizedPath.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult(false); // Path traversal attempt blocked
                }

                return Task.FromResult(File.Exists(normalizedPath));
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public string GetFileUrl(string filePath)
        {
            // Return relative URL for local files
            return $"/{filePath.Replace("\\", "/")}";
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove path information
            fileName = Path.GetFileName(fileName);

            // Remove invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            // Limit length
            if (fileName.Length > 100)
            {
                var extension = Path.GetExtension(fileName);
                fileName = fileName.Substring(0, 100 - extension.Length) + extension;
            }

            return fileName;
        }
    }
}
