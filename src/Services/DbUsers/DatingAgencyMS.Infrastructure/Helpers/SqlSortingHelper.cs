using Common.Filtering.Sorting;

namespace DatingAgencyMS.Infrastructure.Helpers;

public static class SqlSortingHelper
{
    public static string BuildSortingString(this SortingInfo? sortingInfo) => sortingInfo is not null
        ? $"ORDER BY {sortingInfo.SortBy} {sortingInfo.SortDirection} "
        : string.Empty;
}