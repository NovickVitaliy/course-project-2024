using Common.Filtering.Filters;
using Common.Filtering.Filters.DateFilters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.FiltersOptions;

namespace DatingAgencyMS.Infrastructure.Helpers;
/// <summary>
/// ABSOLUTE DANGER BECAUSE OF SQL INJECTION. DO NOT USE IN PRODUCTION. FIX IN FUTURE!!!!!!
/// </summary>
public static class SqlFilterHelper
{
    public static string BuildConditionForInteger(this IntegerFilter integerFilter, string propertyName) =>
        integerFilter.Option switch
        {
            NumberFilterOption.None => string.Empty,
            NumberFilterOption.Equals => $"AND {propertyName} = {integerFilter.Value} ",
            NumberFilterOption.NotEquals => $"AND {propertyName} != {integerFilter.Value} ",
            NumberFilterOption.LessThan => $"AND {propertyName} < {integerFilter.Value} ",
            NumberFilterOption.LessThanOrEquals => $"AND {propertyName} <= {integerFilter.Value} ",
            NumberFilterOption.GreaterThen => $"AND {propertyName} > {integerFilter.Value} ",
            NumberFilterOption.GreaterThenOrEquals => $"AND {propertyName} >= {integerFilter.Value} ",
            NumberFilterOption.Clear => string.Empty,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string BuildConditionForString(this StringFilter stringFilter, string propertyName) =>
        stringFilter.Option switch
        {
            StringFilterOption.None => string.Empty,
            StringFilterOption.Contains => $"AND LOWER({propertyName}) LIKE LOWER('%{stringFilter.Value}%') ",
            StringFilterOption.DoesNotContain => $"AND LOWER({propertyName}) NOT LIKE LOWER('%{stringFilter.Value}%') ",
            StringFilterOption.StartsWith => $"AND LOWER({propertyName}) LIKE LOWER('{stringFilter.Value}%') ",
            StringFilterOption.EndWith => $"AND LOWER({propertyName}) LIKE LOWER('%{stringFilter.Value}') ",
            StringFilterOption.Equals => $"AND LOWER({propertyName}) = LOWER('{stringFilter.Value}') ",
            StringFilterOption.NotEquals => $"AND LOWER({propertyName}) != LOWER('{stringFilter.Value}') ",
            StringFilterOption.Clear => string.Empty,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string BuildConditionForDateOnly(this DateOnlyFilter dateOnlyFilter, string propertyName) =>
        dateOnlyFilter.Option switch
        {
            DateFilterOption.None => string.Empty,
            DateFilterOption.Equals => $"AND {propertyName} = '{dateOnlyFilter.Value:O}' ",
            DateFilterOption.NotEquals => $"AND {propertyName} != '{dateOnlyFilter.Value:O}' ",
            DateFilterOption.LessThan => $"AND {propertyName} < '{dateOnlyFilter.Value:O}' ",
            DateFilterOption.LessThanOrEquals => $"AND {propertyName} <= '{dateOnlyFilter.Value:O}' ",
            DateFilterOption.GreaterThan => $"AND {propertyName} > '{dateOnlyFilter.Value:O}' ",
            DateFilterOption.GreaterThanOrEquals => $"AND {propertyName} >= '{dateOnlyFilter.Value:O}' ",
            DateFilterOption.Clear => string.Empty,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string BuildConditionForDateTime(this DateTimeFilter dateTimeFilter, string propertyName) =>
        dateTimeFilter.Option switch
        {
            DateFilterOption.None => string.Empty,
            DateFilterOption.Equals => $"AND {propertyName} = '{dateTimeFilter.Value:u}' ",
            DateFilterOption.NotEquals => $"AND {propertyName} != '{dateTimeFilter.Value:u}' ",
            DateFilterOption.LessThan => $"AND {propertyName} < '{dateTimeFilter.Value:u}' ",
            DateFilterOption.LessThanOrEquals => $"AND {propertyName} <= '{dateTimeFilter.Value:u}' ",
            DateFilterOption.GreaterThan => $"AND {propertyName} > '{dateTimeFilter.Value:u}' ",
            DateFilterOption.GreaterThanOrEquals => $"AND {propertyName} >= '{dateTimeFilter.Value:u}' ",
            DateFilterOption.Clear => string.Empty,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string BuildConditionForBoolean(this BooleanFilter booleanFilter, string propertyName) =>
        booleanFilter.Option switch
        {
            BooleanFilterOption.None => string.Empty,
            BooleanFilterOption.Equals => $"AND {propertyName} = {booleanFilter.Value}",
            BooleanFilterOption.NotEquals => $"AND {propertyName} != {booleanFilter.Value}",
            BooleanFilterOption.Clear => string.Empty,
            _ => throw new ArgumentOutOfRangeException()
        };
}