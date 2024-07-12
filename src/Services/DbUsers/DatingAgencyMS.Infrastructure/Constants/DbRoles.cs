using DatingAgencyMS.Domain.Models;

namespace DatingAgencyMS.Infrastructure.Constants;

public static class DbRolesInfo
{
    public static string GetGrantPrivilegesTemplateStringForRole(DbRoles role) =>
        role switch
        {
            DbRoles.Owner => "grant all privileges on database \"datingAgencyDb\" TO {0};",
            DbRoles.Admin => "grant admin to {0};",
            DbRoles.Operator => "grant operator to {0};",
            DbRoles.Guest => "grant guest to {0};",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
}