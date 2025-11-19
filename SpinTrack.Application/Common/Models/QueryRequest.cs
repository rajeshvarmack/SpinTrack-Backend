namespace SpinTrack.Application.Common.Models
{
    /// <summary>
    /// Query request with pagination, filtering, sorting, and search
    /// </summary>
    public class QueryRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public List<ColumnFilter>? Filters { get; set; }
        public List<SortColumn>? SortColumns { get; set; }
        public ExportFormat? ExportFormat { get; set; }
        public bool ExportAll { get; set; }
    }
}
