namespace SpinTrack.Application.Common.Services
{
    /// <summary>
    /// Service for exporting data to CSV format
    /// </summary>
    public interface ICsvExportService
    {
        byte[] ExportToCsv<T>(IEnumerable<T> data, Dictionary<string, Func<T, object>>? columnMappings = null);
    }
}
