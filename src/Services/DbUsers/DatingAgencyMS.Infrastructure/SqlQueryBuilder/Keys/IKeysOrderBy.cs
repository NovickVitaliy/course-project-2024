using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Common;

namespace DatingAgencyMS.Infrastructure.SqlQueryBuilder.Keys;

public interface IKeysOrderBy
{
    IPagingSqlQueryBuilder OrderBy(string orderBy, string orderDirection);
    IPagingSqlQueryBuilder OrderById();
}