namespace SpinTrack.Application.Common.Services
{
    /// <summary>
    /// Service for exporting data to Excel format
    /// </summary>
    public interface IExcelExportService
    {
        /// <summary>
        /// Exports a collection of items to Excel format
        /// </summary>
        /// <typeparam name="T">Type of items to export</typeparam>
        /// <param name="items">Collection of items</param>
        /// <param name="columnMappings">Dictionary mapping column names to value extractors</param>
        /// <returns>Excel file content as byte array</returns>
        byte[] ExportToExcel<T>(IEnumerable<T> items, Dictionary<string, Func<T, object>> columnMappings);
    }
}
