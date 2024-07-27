using System.Globalization;
using BlazorBootstrap;
using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.FiltersOptions;

namespace DatingAgencyMS.Client.Helpers;

public static class FilterHelper
{
    public static NumberFilterOption ToNumberFilterOption(this FilterOperator filterOperator) =>
        filterOperator switch
        {
            FilterOperator.None => NumberFilterOption.None,
            FilterOperator.Equals => NumberFilterOption.Equals,
            FilterOperator.NotEquals => NumberFilterOption.NotEquals,
            FilterOperator.LessThan => NumberFilterOption.LessThan,
            FilterOperator.LessThanOrEquals => NumberFilterOption.LessThanOrEquals,
            FilterOperator.GreaterThan => NumberFilterOption.GreaterThen,
            FilterOperator.GreaterThanOrEquals => NumberFilterOption.GreaterThenOrEquals,
            FilterOperator.Clear => NumberFilterOption.Clear,
            _ => throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null)
        };

    public static StringFilterOption ToStringFilterOption(this FilterOperator filterOperator) =>
        filterOperator switch
        {
            FilterOperator.None => StringFilterOption.None,
            FilterOperator.Equals => StringFilterOption.Equals,
            FilterOperator.NotEquals => StringFilterOption.NotEquals,
            FilterOperator.Contains => StringFilterOption.Contains,
            FilterOperator.StartsWith => StringFilterOption.StartsWith,
            FilterOperator.EndsWith => StringFilterOption.EndWith,
            FilterOperator.DoesNotContain => StringFilterOption.DoesNotContain,
            FilterOperator.Clear => StringFilterOption.Clear,
            _ => throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null)
        };

    public static BooleanFilterOption ToBooleanFilterOptions(this FilterOperator filterOperator) =>
        filterOperator switch
        {
            FilterOperator.Equals => BooleanFilterOption.Equals,
            FilterOperator.NotEquals => BooleanFilterOption.NotEquals,
            FilterOperator.Clear => BooleanFilterOption.Clear
        };

    public static DateFilterOption ToDateFilterOption(this FilterOperator filterOperator) =>
        filterOperator switch
        {   
            FilterOperator.Equals => DateFilterOption.Equals,
            FilterOperator.NotEquals => DateFilterOption.NotEquals,
            FilterOperator.LessThan => DateFilterOption.LessThan,
            FilterOperator.LessThanOrEquals => DateFilterOption.LessThanOrEquals,
            FilterOperator.GreaterThan => DateFilterOption.GreaterThan,
            FilterOperator.GreaterThanOrEquals => DateFilterOption.GreaterThanOrEquals,
            FilterOperator.Clear => DateFilterOption.Clear
        };

    public static IntegerFilter GetIntegerFilter(this IEnumerable<FilterItem> filters, string propertyName)
    {
        var filter = filters.FirstOrDefault(x => x.PropertyName == propertyName);
        return filter is not null
            ? new IntegerFilter(int.Parse(filter.Value), filter.Operator.ToNumberFilterOption())
            : new IntegerFilter(default, NumberFilterOption.None);
    }

    public static StringFilter GetStringFilter(this IEnumerable<FilterItem> filters, string propertyName)
    {
        var filter = filters.FirstOrDefault(x => x.PropertyName == propertyName);
        return filter is not null
            ? new StringFilter(filter.Value, filter.Operator.ToStringFilterOption())
            : new StringFilter(string.Empty, StringFilterOption.None);
    }

    public static DateOnlyFilter GetDateOnlyFilter(this IEnumerable<FilterItem> filters, string propertyName)
    {
        var filter = filters.FirstOrDefault(x => x.PropertyName == propertyName);
        return filter is not null
            ? new DateOnlyFilter(DateOnly.Parse(filter.Value, CultureInfo.InvariantCulture), filter.Operator.ToDateFilterOption())
            : new DateOnlyFilter(default, DateFilterOption.None);
    }

    public static DateTimeFilter GetDateTimeFilter(this IEnumerable<FilterItem> filters, string propertyName)
    {
        var filter = filters.FirstOrDefault(x => x.PropertyName == propertyName);
        return filter is not null
            ? new DateTimeFilter(DateTime.Parse(filter.Value, CultureInfo.InvariantCulture), filter.Operator.ToDateFilterOption())
            : new DateTimeFilter(default, DateFilterOption.None);
    }

    public static BooleanFilter GetBooleanFilter(this IEnumerable<FilterItem> filters, string propertyName)
    {
        var filter = filters.FirstOrDefault(x => x.PropertyName == propertyName);
        return filter is not null
            ? new BooleanFilter(bool.Parse(filter.Value), filter.Operator.ToBooleanFilterOptions())
            : new BooleanFilter(default, BooleanFilterOption.None);
    }
}