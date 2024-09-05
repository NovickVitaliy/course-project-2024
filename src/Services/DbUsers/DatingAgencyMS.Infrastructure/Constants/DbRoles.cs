using DatingAgencyMS.Domain.Models.DbManagement;

namespace DatingAgencyMS.Infrastructure.Constants;

public static class DbRolesInfo
{
    public static string GetGrantRoleForUserString(DbRoles role, string login) =>
        role switch
        {
            DbRoles.Owner => $"GRANT ALL PRIVILEGES ON DATABASE \"datingAgencyDb\" TO \"{login}\";",
            DbRoles.Admin => $"GRANT admin TO \"{login}\";",
            DbRoles.Operator => $"GRANT operator TO \"{login}\";",
            DbRoles.Guest => $"GRANT guest TO \"{login}\";",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };

    public static string GetRevokeRoleFromUserString(DbRoles role, string login)
        => role switch
        {
            DbRoles.Owner => $"REVOKE owner FROM \"{login}\";",
            DbRoles.Admin => $"REVOKE admin FROM \"{login}\";",
            DbRoles.Operator => $"REVOKE operator FROM \"{login}\";",
            DbRoles.Guest => $"REVOKE guest FROM \"{login}\";",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
}