namespace SpinTrack.Application.Common.Models
{
    /// <summary>
    /// Represents a column filter with operator and value
    /// </summary>
    public class ColumnFilter
    {
        public string ColumnName { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; }
        public string? Value { get; set; }
        public string? ValueTo { get; set; } // For Between operator (range end)
        public List<string>? Values { get; set; } // For In/NotIn operators
    }
}
