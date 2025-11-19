using SpinTrack.Application.Common.Services;
using System.Text;

namespace SpinTrack.Infrastructure.Services
{
    /// <summary>
    /// CSV export service implementation
    /// </summary>
    public class CsvExportService : ICsvExportService
    {
        public byte[] ExportToCsv<T>(IEnumerable<T> data, Dictionary<string, Func<T, object>>? columnMappings = null)
        {
            var sb = new StringBuilder();

            if (columnMappings != null && columnMappings.Any())
            {
                // Write header
                sb.AppendLine(string.Join(",", columnMappings.Keys.Select(EscapeCsvValue)));

                // Write data rows
                foreach (var item in data)
                {
                    var values = columnMappings.Values.Select(func => EscapeCsvValue(func(item)?.ToString() ?? string.Empty));
                    sb.AppendLine(string.Join(",", values));
                }
            }
            else
            {
                // Use reflection to get properties
                var properties = typeof(T).GetProperties();

                // Write header
                sb.AppendLine(string.Join(",", properties.Select(p => EscapeCsvValue(p.Name))));

                // Write data rows
                foreach (var item in data)
                {
                    var values = properties.Select(p => EscapeCsvValue(p.GetValue(item)?.ToString() ?? string.Empty));
                    sb.AppendLine(string.Join(",", values));
                }
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // If value contains comma, newline, or double quote, wrap it in quotes
            if (value.Contains(',') || value.Contains('\n') || value.Contains('"'))
            {
                // Escape double quotes by doubling them
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }
}
