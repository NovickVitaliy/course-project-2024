namespace DatingAgencyMS.Infrastructure.SqlQueryBuilder.Keys;

public interface IKeysCondition
{
    bool HasConditionChainingStarted { get; set; }
    IKeysCondition StartConditionClause();
    IKeysCondition WithLoginLike(string login);
    IKeysCondition WithIdEqual(int id);
    IKeysCondition WithRoleLike(string role);
    IKeysCondition And();
    IKeysCondition Or();
    IKeysCondition StartGroup();
    IKeysCondition FinishGroup();
    IKeysOrderBy EndConditionClause();
}