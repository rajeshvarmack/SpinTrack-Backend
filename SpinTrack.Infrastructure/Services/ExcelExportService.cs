using OfficeOpenXml;
using OfficeOpenXml.Style;
using SpinTrack.Application.Common.Services;
using System.Drawing;

namespace SpinTrack.Infrastructure.Services
{
    /// <summary>
    /// Service for exporting data to Excel format using EPPlus
    /// </summary>
    public class ExcelExportService : IExcelExportService
    {
        public ExcelExportService()
        {
            // Set EPPlus license context (NonCommercial or Commercial)
            // For EPPlus 8+, use ExcelPackage.LicenseContext
#pragma warning disable CS0618 // Type or member is obsolete
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Exports a collection of items to Excel format
        /// </summary>
        public byte[] ExportToExcel<T>(IEnumerable<T> items, Dictionary<string, Func<T, object>> columnMappings)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Data");

            // Get column headers
            var headers = columnMappings.Keys.ToList();

            // Write headers
            for (int col = 0; col < headers.Count; col++)
            {
                worksheet.Cells[1, col + 1].Value = headers[col];
            }

            // Style headers
            using (var headerRange = worksheet.Cells[1, 1, 1, headers.Count])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); // Blue
                headerRange.Style.Font.Color.SetColor(Color.White);
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Write data rows
            var itemsList = items.ToList();
            for (int row = 0; row < itemsList.Count; row++)
            {
                var item = itemsList[row];
                for (int col = 0; col < headers.Count; col++)
                {
                    var header = headers[col];
                    var value = columnMappings[header](item);
                    worksheet.Cells[row + 2, col + 1].Value = value;
                }
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Add borders to all cells
            using (var dataRange = worksheet.Cells[1, 1, itemsList.Count + 1, headers.Count])
            {
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            // Freeze header row
            worksheet.View.FreezePanes(2, 1);

            // Return Excel file as byte array
            return package.GetAsByteArray();
        }
    }
}
