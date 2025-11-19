namespace SpinTrack.Application.Common.Settings
{
    /// <summary>
    /// File storage configuration settings
    /// </summary>
    public class FileStorageSettings
    {
        public string Provider { get; set; } = "Local"; // Local or AzureBlob
        public string LocalBasePath { get; set; } = "wwwroot/uploads";
        public string AzureConnectionString { get; set; } = string.Empty;
        public string AzureContainerName { get; set; } = "profile-pictures";
        public int MaxFileSizeInMB { get; set; } = 5;
        public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    }
}
