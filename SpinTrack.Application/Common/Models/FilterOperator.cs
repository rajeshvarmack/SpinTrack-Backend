namespace SpinTrack.Application.Common.Models
{
    /// <summary>
    /// Filter operators for column filtering
    /// </summary>
    public enum FilterOperator
    {
        // String operators
        Equals,
        NotEquals,
        Contains,
        NotContains,
        StartsWith,
        EndsWith,
        IsEmpty,
        IsNotEmpty,
        
        // Numeric/Date operators
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Between,
        
        // Collection operators
        In,
        NotIn,
        
        // Null operators
        IsNull,
        IsNotNull
    }
}
