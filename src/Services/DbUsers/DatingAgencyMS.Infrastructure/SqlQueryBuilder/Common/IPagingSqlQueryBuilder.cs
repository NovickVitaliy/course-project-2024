using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Keys;

namespace DatingAgencyMS.Infrastructure.SqlQueryBuilder.Common;

public interface IPagingSqlQueryBuilder
{
    ISqlFinishedQuery WithPaging(int page, int size);
    ISqlFinishedQuery NoPaging();
}