namespace Common.Filtering.Pagination;

public record PaginationInfo(int PageNumber, int PageSize);


/// <summary>
/// Should be in a separate assembly for different dbs. Come in some future. or maybe not :))))))))))))))))))
/// </summary>
public static class PaginationHelper
{
    public static string ToPostgreSqlPaginationString(this PaginationInfo paginationInfo)
    {
        var skip = (paginationInfo.PageNumber - 1) * paginationInfo.PageSize;
        var take = paginationInfo.PageSize;
        
        return $"OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
    }
}