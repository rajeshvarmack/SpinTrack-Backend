using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SpinTrack.Application.Common.Services;
using SpinTrack.Application.Common.Settings;

namespace SpinTrack.Infrastructure.Services
{
    /// <summary>
    /// Azure Blob Storage service implementation with validation
    /// </summary>
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly FileStorageSettings _settings;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(IOptions<FileStorageSettings> settings)
        {
            _settings = settings.Value;

            if (string.IsNullOrWhiteSpace(_settings.AzureConnectionString))
            {
                throw new InvalidOperationException("Azure Storage connection string is not configured");
            }

            _blobServiceClient = new BlobServiceClient(_settings.AzureConnectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_settings.AzureContainerName);

            // Create container if it doesn't exist
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
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

            // Sanitize and generate unique blob name
            var sanitizedFileName = SanitizeFileName(fileName);
            var uniqueBlobName = $"{Guid.NewGuid()}{Path.GetExtension(sanitizedFileName)}";

            var blobClient = _containerClient.GetBlobClient(uniqueBlobName);

            // Set content type
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            // Upload to Azure Blob Storage
            await blobClient.UploadAsync(
                fileStream,
                new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                },
                cancellationToken);

            // Return blob URI
            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                // Extract blob name from URI
                var blobName = GetBlobNameFromUrl(filePath);
                var blobClient = _containerClient.GetBlobClient(blobName);

                var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
                return response.Value;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Stream?> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobName = GetBlobNameFromUrl(filePath);
                var blobClient = _containerClient.GetBlobClient(blobName);

                if (await blobClient.ExistsAsync(cancellationToken))
                {
                    var response = await blobClient.DownloadAsync(cancellationToken);
                    return response.Value.Content;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobName = GetBlobNameFromUrl(filePath);
                var blobClient = _containerClient.GetBlobClient(blobName);
                return await blobClient.ExistsAsync(cancellationToken);
            }
            catch
            {
                return false;
            }
        }

        public string GetFileUrl(string filePath)
        {
            // If already a full URL, return as is
            if (filePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                filePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return filePath;
            }

            // Otherwise, construct full URL
            var blobClient = _containerClient.GetBlobClient(filePath);
            return blobClient.Uri.ToString();
        }

        private string GetBlobNameFromUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                // Extract blob name from URI
                return uri.Segments.Last();
            }

            // If not a URI, assume it's already a blob name
            return url;
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
