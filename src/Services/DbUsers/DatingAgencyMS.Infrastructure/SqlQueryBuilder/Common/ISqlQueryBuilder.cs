namespace DatingAgencyMS.Infrastructure.SqlQueryBuilder.Common;

public interface ISqlQueryBuilder<out T>
{
    T Clear();
}