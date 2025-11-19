using FluentValidation;
using SpinTrack.Application.Common.Models;

namespace SpinTrack.Application.Common.Validators
{
    /// <summary>
    /// Validator for QueryRequest
    /// </summary>
    public class QueryRequestValidator : AbstractValidator<QueryRequest>
    {
        public QueryRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

            RuleForEach(x => x.Filters)
                .SetValidator(new ColumnFilterValidator())
                .When(x => x.Filters != null && x.Filters.Any());

            RuleForEach(x => x.SortColumns)
                .SetValidator(new SortColumnValidator())
                .When(x => x.SortColumns != null && x.SortColumns.Any());
        }
    }

    public class ColumnFilterValidator : AbstractValidator<ColumnFilter>
    {
        public ColumnFilterValidator()
        {
            RuleFor(x => x.ColumnName)
                .NotEmpty().WithMessage("Column name is required");

            RuleFor(x => x.Operator)
                .IsInEnum().WithMessage("Invalid filter operator");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Filter value is required")
                .When(x => x.Operator != FilterOperator.IsNull 
                    && x.Operator != FilterOperator.IsNotNull
                    && x.Operator != FilterOperator.IsEmpty
                    && x.Operator != FilterOperator.IsNotEmpty
                    && x.Operator != FilterOperator.In 
                    && x.Operator != FilterOperator.NotIn
                    && x.Operator != FilterOperator.Between);

            RuleFor(x => x.Values)
                .NotEmpty().WithMessage("Filter values are required for In/NotIn operators")
                .When(x => x.Operator == FilterOperator.In || x.Operator == FilterOperator.NotIn);

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("From value is required for Between operator")
                .When(x => x.Operator == FilterOperator.Between);

            RuleFor(x => x.ValueTo)
                .NotEmpty().WithMessage("To value is required for Between operator")
                .When(x => x.Operator == FilterOperator.Between);
        }
    }

    public class SortColumnValidator : AbstractValidator<SortColumn>
    {
        public SortColumnValidator()
        {
            RuleFor(x => x.ColumnName)
                .NotEmpty().WithMessage("Column name is required");

            RuleFor(x => x.Direction)
                .IsInEnum().WithMessage("Invalid sort direction");
        }
    }
}
