using SpinTrack.Application.Common.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace SpinTrack.Application.Common.Helpers
{
    /// <summary>
    /// Builds dynamic LINQ expressions for filtering
    /// </summary>
    public static class FilterExpressionBuilder
    {
        /// <summary>
        /// Applies a list of filters to an IQueryable
        /// </summary>
        public static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, List<ColumnFilter> filters)
        {
            foreach (var filter in filters)
            {
                var predicate = BuildPredicate<T>(filter);
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }
            }
            return query;
        }

        public static Expression<Func<T, bool>>? BuildPredicate<T>(ColumnFilter filter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetProperty<T>(parameter, filter.ColumnName);

            if (property == null)
                return null;

            Expression? expression = filter.Operator switch
            {
                FilterOperator.Equals => BuildEquals(property, filter.Value),
                FilterOperator.NotEquals => BuildNotEquals(property, filter.Value),
                FilterOperator.Contains => BuildContains(property, filter.Value),
                FilterOperator.NotContains => BuildNotContains(property, filter.Value),
                FilterOperator.StartsWith => BuildStartsWith(property, filter.Value),
                FilterOperator.EndsWith => BuildEndsWith(property, filter.Value),
                FilterOperator.IsEmpty => BuildIsEmpty(property),
                FilterOperator.IsNotEmpty => BuildIsNotEmpty(property),
                FilterOperator.GreaterThan => BuildGreaterThan(property, filter.Value),
                FilterOperator.GreaterThanOrEqual => BuildGreaterThanOrEqual(property, filter.Value),
                FilterOperator.LessThan => BuildLessThan(property, filter.Value),
                FilterOperator.LessThanOrEqual => BuildLessThanOrEqual(property, filter.Value),
                FilterOperator.Between => BuildBetween(property, filter.Value, filter.ValueTo),
                FilterOperator.In => BuildIn(property, filter.Values),
                FilterOperator.NotIn => BuildNotIn(property, filter.Values),
                FilterOperator.IsNull => BuildIsNull(property),
                FilterOperator.IsNotNull => BuildIsNotNull(property),
                _ => null
            };

            if (expression == null)
                return null;

            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }

        private static MemberExpression? GetProperty<T>(Expression parameter, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName, 
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            return property != null ? Expression.Property(parameter, property) : null;
        }

        private static Expression? BuildEquals(MemberExpression property, string? value)
        {
            if (value == null) return null;
            var constant = GetConstantExpression(property.Type, value);
            return constant != null ? Expression.Equal(property, constant) : null;
        }

        private static Expression? BuildNotEquals(MemberExpression property, string? value)
        {
            if (value == null) return null;
            var constant = GetConstantExpression(property.Type, value);
            return constant != null ? Expression.NotEqual(property, constant) : null;
        }

        private static Expression? BuildContains(MemberExpression property, string? value)
        {
            if (value == null || property.Type != typeof(string)) return null;
            
            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var constant = Expression.Constant(value, typeof(string));
            
            return method != null ? Expression.Call(property, method, constant) : null;
        }

        private static Expression? BuildNotContains(MemberExpression property, string? value)
        {
            if (value == null || property.Type != typeof(string)) return null;
            
            var containsExpression = BuildContains(property, value);
            return containsExpression != null ? Expression.Not(containsExpression) : null;
        }

        private static Expression? BuildStartsWith(MemberExpression property, string? value)
        {
            if (value == null || property.Type != typeof(string)) return null;
            
            var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var constant = Expression.Constant(value, typeof(string));
            
            return method != null ? Expression.Call(property, method, constant) : null;
        }

        private static Expression? BuildEndsWith(MemberExpression property, string? value)
        {
            if (value == null || property.Type != typeof(string)) return null;
            
            var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            var constant = Expression.Constant(value, typeof(string));
            
            return method != null ? Expression.Call(property, method, constant) : null;
        }

        private static Expression? BuildIsEmpty(MemberExpression property)
        {
            if (property.Type != typeof(string)) return null;

            // Check for null or empty: string.IsNullOrEmpty(property)
            var isNullOrEmptyMethod = typeof(string).GetMethod("IsNullOrEmpty", new[] { typeof(string) });
            
            return isNullOrEmptyMethod != null ? Expression.Call(isNullOrEmptyMethod, property) : null;
        }

        private static Expression? BuildIsNotEmpty(MemberExpression property)
        {
            if (property.Type != typeof(string)) return null;

            var isEmptyExpression = BuildIsEmpty(property);
            return isEmptyExpression != null ? Expression.Not(isEmptyExpression) : null;
        }

        private static Expression? BuildGreaterThan(MemberExpression property, string? value)
        {
            if (value == null) return null;
            var constant = GetConstantExpression(property.Type, value);
            return constant != null ? Expression.GreaterThan(property, constant) : null;
        }

        private static Expression? BuildGreaterThanOrEqual(MemberExpression property, string? value)
        {
            if (value == null) return null;
            var constant = GetConstantExpression(property.Type, value);
            return constant != null ? Expression.GreaterThanOrEqual(property, constant) : null;
        }

        private static Expression? BuildLessThan(MemberExpression property, string? value)
        {
            if (value == null) return null;
            var constant = GetConstantExpression(property.Type, value);
            return constant != null ? Expression.LessThan(property, constant) : null;
        }

        private static Expression? BuildLessThanOrEqual(MemberExpression property, string? value)
        {
            if (value == null) return null;
            var constant = GetConstantExpression(property.Type, value);
            return constant != null ? Expression.LessThanOrEqual(property, constant) : null;
        }

        private static Expression? BuildBetween(MemberExpression property, string? valueFrom, string? valueTo)
        {
            if (valueFrom == null || valueTo == null) return null;

            var constantFrom = GetConstantExpression(property.Type, valueFrom);
            var constantTo = GetConstantExpression(property.Type, valueTo);

            if (constantFrom == null || constantTo == null) return null;

            // property >= valueFrom AND property <= valueTo
            var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, constantFrom);
            var lessThanOrEqual = Expression.LessThanOrEqual(property, constantTo);

            return Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
        }

        private static Expression? BuildIn(MemberExpression property, List<string>? values)
        {
            if (values == null || !values.Any()) return null;

            var constants = values
                .Select(v => GetConstantExpression(property.Type, v))
                .Where(c => c != null)
                .ToList();

            if (!constants.Any()) return null;

            Expression? result = null;
            foreach (var constant in constants)
            {
                var equals = Expression.Equal(property, constant!);
                result = result == null ? equals : Expression.OrElse(result, equals);
            }

            return result;
        }

        private static Expression? BuildNotIn(MemberExpression property, List<string>? values)
        {
            var inExpression = BuildIn(property, values);
            return inExpression != null ? Expression.Not(inExpression) : null;
        }

        private static Expression BuildIsNull(MemberExpression property)
        {
            return Expression.Equal(property, Expression.Constant(null, property.Type));
        }

        private static Expression BuildIsNotNull(MemberExpression property)
        {
            return Expression.NotEqual(property, Expression.Constant(null, property.Type));
        }

        private static ConstantExpression? GetConstantExpression(Type type, string value)
        {
            try
            {
                var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

                if (underlyingType == typeof(string))
                    return Expression.Constant(value, type);

                if (underlyingType == typeof(int))
                    return Expression.Constant(int.Parse(value), type);

                if (underlyingType == typeof(long))
                    return Expression.Constant(long.Parse(value), type);

                if (underlyingType == typeof(decimal))
                    return Expression.Constant(decimal.Parse(value), type);

                if (underlyingType == typeof(double))
                    return Expression.Constant(double.Parse(value), type);

                if (underlyingType == typeof(float))
                    return Expression.Constant(float.Parse(value), type);

                if (underlyingType == typeof(bool))
                    return Expression.Constant(bool.Parse(value), type);

                if (underlyingType == typeof(DateTime))
                    return Expression.Constant(DateTime.Parse(value), type);

                if (underlyingType == typeof(DateTimeOffset))
                    return Expression.Constant(DateTimeOffset.Parse(value), type);

                if (underlyingType == typeof(DateOnly))
                    return Expression.Constant(DateOnly.Parse(value), type);

                if (underlyingType == typeof(Guid))
                    return Expression.Constant(Guid.Parse(value), type);

                if (underlyingType.IsEnum)
                    return Expression.Constant(Enum.Parse(underlyingType, value), type);

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
