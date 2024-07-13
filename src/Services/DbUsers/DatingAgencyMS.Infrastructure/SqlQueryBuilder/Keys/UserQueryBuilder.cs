using System.Text;
using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Common;
using Microsoft.Extensions.Primitives;

namespace DatingAgencyMS.Infrastructure.SqlQueryBuilder.Keys;

public abstract class BaseKeysSqlQueryBuilder : ISqlQueryBuilder<IKeysCondition>
{
    protected readonly StringBuilder _sb = new("SELECT * FROM keys ");
    
    public IKeysCondition Clear()
    {
        _sb.Clear();
        return new Builder();
    }
}

public static class UserQueryBuilder
{
    public static IKeysCondition GetUserQueryBuilder()
    {
        return new Builder();
    }
}

public class Builder : BaseKeysSqlQueryBuilder, IKeysCondition, IKeysOrderBy, IPagingSqlQueryBuilder, IKeysConditionStart, ISqlFinishedQuery
{
    public ISqlFinishedQuery WithPaging(int page, int size)
    {
        var skip = (page - 1) * size;
        _sb.Append($"OFFSET {skip} ROWS FETCH NEXT {size} ROWS ONLY");
        return this;
    }

    public ISqlFinishedQuery NoPaging()
    {
        return this;
    }

    public bool HasConditionChainingStarted { get; set; }

    public IKeysCondition StartConditionClause()
    {
        _sb.Append("WHERE id IS NOT NULL ");
        return this;
    }

    public IKeysCondition WithLoginLike(string login)
    {
        _sb.Append($"LOWER(login) LIKE LOWER('%{login}%') ");
        return this;
    }
    

    public IKeysCondition WithIdEqual(int id)
    {
        _sb.Append($"id = {id} ");
        return this;
    }

    public IKeysCondition WithRoleLike(string role)
    {
        _sb.Append($"LOWER(role) LIKE LOWER('%{role}%') ");
        return this;
    }
    

    public IKeysCondition And()
    {
        _sb.Append("AND ");
        return this;
    }

    public IKeysCondition Or()
    {
        _sb.Append("OR ");
        return this;
    }

    public IKeysCondition StartGroup()
    {
        _sb.Append("( ");
        return this;
    }

    public IKeysCondition FinishGroup()
    {
        _sb.Append(" ) ");
        return this;
    }

    public IKeysOrderBy EndConditionClause()
    {
        _sb.Append("ORDER BY ");
        return this;
    }

    public IPagingSqlQueryBuilder OrderBy(string orderBy, string orderDirection)
    {
        _sb.Append($"{orderBy} {orderDirection} ");
        return this;
    }

    public IPagingSqlQueryBuilder OrderById()
    {
        _sb.Append("id ");
        return this;
    }

    public string ToSql()
    {
        return _sb.ToString();
    }
}