namespace SpinTrack.Application.Common.Models
{
    /// <summary>
    /// Represents a column sort configuration
    /// </summary>
    public class SortColumn
    {
        public string ColumnName { get; set; } = string.Empty;
        public SortDirection Direction { get; set; } = SortDirection.Ascending;
    }
}
