using BlazorBootstrap;

namespace DatingAgencyMS.Client.Helpers;

public static class SortingHelper
{
    public static string GetSortDirection<T>(this SortingItem<T> sortingItem)
        => sortingItem.SortDirection switch {
            SortDirection.None => string.Empty,
            SortDirection.Ascending => "ASC",
            SortDirection.Descending => "DESC",
            _ => throw new ArgumentOutOfRangeException()
        };
}