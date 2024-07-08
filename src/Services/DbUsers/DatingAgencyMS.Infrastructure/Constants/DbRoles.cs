namespace DatingAgencyMS.Infrastructure.Constants;

public static class DbRoles
{
    public static readonly IReadOnlyList<string> Roles = ["owner", "admin", "operator", "guest"];

    public static bool EnsureRoleExists(string role) => Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase);

    public static string GetGrantPrivilegesTemplateStringForRole(string role) =>
        role switch
        {
            "owner" => "grant all privileges on database \"datingAgencyDb\" TO {0};",
            "admin" => "grant admin to {0};",
            "operator" => "grant operator to {0};",
            "guest" => "grant guest to {0};",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
}