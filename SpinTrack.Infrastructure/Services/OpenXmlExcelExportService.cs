using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SpinTrack.Application.Common.Services;

namespace SpinTrack.Infrastructure.Services
{
    /// <summary>
    /// Service for exporting data to Excel format using Microsoft's Open XML SDK (no third-party dependencies)
    /// This is the official Microsoft way to create Excel files
    /// </summary>
    public class OpenXmlExcelExportService : IExcelExportService
    {
        /// <summary>
        /// Exports a collection of items to Excel format using Open XML SDK
        /// </summary>
        public byte[] ExportToExcel<T>(IEnumerable<T> items, Dictionary<string, Func<T, object>> columnMappings)
        {
            using var memoryStream = new MemoryStream();
            
            // Create SpreadsheetDocument
            using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                // Add WorkbookPart
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Add WorksheetPart
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                // Add Stylesheet for formatting
                var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = CreateStylesheet();
                stylesPart.Stylesheet.Save();

                // Create sheets
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                var sheet = new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Data"
                };
                sheets.Append(sheet);

                // Get column headers
                var headers = columnMappings.Keys.ToList();

                // Create SheetData
                var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                // Add header row with styling
                var headerRow = new Row { RowIndex = 1 };
                for (int col = 0; col < headers.Count; col++)
                {
                    var cell = new Cell
                    {
                        CellReference = GetCellReference(1, col),
                        DataType = CellValues.String,
                        CellValue = new CellValue(headers[col]),
                        StyleIndex = 1 // Header style
                    };
                    headerRow.Append(cell);
                }
                sheetData.Append(headerRow);

                // Add data rows
                var itemsList = items.ToList();
                uint rowIndex = 2;
                foreach (var item in itemsList)
                {
                    var dataRow = new Row { RowIndex = rowIndex };
                    for (int col = 0; col < headers.Count; col++)
                    {
                        var header = headers[col];
                        var value = columnMappings[header](item);
                        var cell = CreateCell(rowIndex, col, value);
                        dataRow.Append(cell);
                    }
                    sheetData.Append(dataRow);
                    rowIndex++;
                }

                // Add column width settings
                var columns = new Columns();
                for (uint col = 1; col <= headers.Count; col++)
                {
                    columns.Append(new Column
                    {
                        Min = col,
                        Max = col,
                        Width = 15,
                        CustomWidth = true
                    });
                }
                worksheetPart.Worksheet.InsertBefore(columns, sheetData);

                // Save worksheet
                worksheetPart.Worksheet.Save();
                workbookPart.Workbook.Save();
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Creates a stylesheet with header and data cell styles
        /// </summary>
        private Stylesheet CreateStylesheet()
        {
            var stylesheet = new Stylesheet();

            // Fonts
            var fonts = new Fonts { Count = 2 };
            
            // Font 0: Default
            fonts.Append(new Font
            {
                FontSize = new FontSize { Val = 11 },
                FontName = new FontName { Val = "Calibri" }
            });

            // Font 1: Bold for headers
            fonts.Append(new Font
            {
                FontSize = new FontSize { Val = 11 },
                FontName = new FontName { Val = "Calibri" },
                Bold = new Bold(),
                Color = new Color { Rgb = "FFFFFFFF" } // White
            });

            stylesheet.Append(fonts);

            // Fills
            var fills = new Fills { Count = 3 };
            
            // Fill 0: Default (required)
            fills.Append(new Fill
            {
                PatternFill = new PatternFill { PatternType = PatternValues.None }
            });

            // Fill 1: Gray (required)
            fills.Append(new Fill
            {
                PatternFill = new PatternFill { PatternType = PatternValues.Gray125 }
            });

            // Fill 2: Blue for headers
            fills.Append(new Fill
            {
                PatternFill = new PatternFill
                {
                    PatternType = PatternValues.Solid,
                    ForegroundColor = new ForegroundColor { Rgb = "FF4F81BD" }, // Blue
                    BackgroundColor = new BackgroundColor { Indexed = 64 }
                }
            });

            stylesheet.Append(fills);

            // Borders
            var borders = new Borders { Count = 2 };
            
            // Border 0: No border (default)
            borders.Append(new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            });

            // Border 1: All borders
            borders.Append(new Border
            {
                LeftBorder = new LeftBorder { Style = BorderStyleValues.Thin },
                RightBorder = new RightBorder { Style = BorderStyleValues.Thin },
                TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            });

            stylesheet.Append(borders);

            // Cell formats
            var cellFormats = new CellFormats { Count = 2 };
            
            // CellFormat 0: Default
            cellFormats.Append(new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 1, // With borders
                ApplyBorder = true
            });

            // CellFormat 1: Header style
            cellFormats.Append(new CellFormat
            {
                FontId = 1, // Bold white font
                FillId = 2, // Blue fill
                BorderId = 1, // With borders
                ApplyFont = true,
                ApplyFill = true,
                ApplyBorder = true,
                Alignment = new Alignment
                {
                    Horizontal = HorizontalAlignmentValues.Center,
                    Vertical = VerticalAlignmentValues.Center
                },
                ApplyAlignment = true
            });

            stylesheet.Append(cellFormats);

            return stylesheet;
        }

        /// <summary>
        /// Creates a cell with appropriate data type and value
        /// </summary>
        private Cell CreateCell(uint rowIndex, int columnIndex, object value)
        {
            var cell = new Cell
            {
                CellReference = GetCellReference(rowIndex, columnIndex),
                StyleIndex = 0 // Data style
            };

            if (value == null)
            {
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(string.Empty);
            }
            else if (value is int || value is long || value is decimal || value is double || value is float)
            {
                cell.DataType = CellValues.Number;
                cell.CellValue = new CellValue(value.ToString() ?? "0");
            }
            else if (value is DateTime dateTime)
            {
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else if (value is bool boolValue)
            {
                cell.DataType = CellValues.Boolean;
                cell.CellValue = new CellValue(boolValue ? "1" : "0");
            }
            else
            {
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(value.ToString() ?? string.Empty);
            }

            return cell;
        }

        /// <summary>
        /// Gets cell reference (e.g., "A1", "B2", "AA10")
        /// </summary>
        private string GetCellReference(uint rowIndex, int columnIndex)
        {
            var columnName = GetColumnName(columnIndex);
            return $"{columnName}{rowIndex}";
        }

        /// <summary>
        /// Converts column index to Excel column name (0 -> A, 1 -> B, 26 -> AA, etc.)
        /// </summary>
        private string GetColumnName(int columnIndex)
        {
            var columnName = string.Empty;
            var index = columnIndex;

            while (index >= 0)
            {
                columnName = (char)('A' + (index % 26)) + columnName;
                index = (index / 26) - 1;
            }

            return columnName;
        }
    }
}
