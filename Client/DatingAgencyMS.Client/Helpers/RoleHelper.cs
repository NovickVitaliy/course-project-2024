using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Helpers;

public static class RoleHelper
{
    private const DbRoles RolesAllowedToCreateEntities = DbRoles.Owner | DbRoles.Admin | DbRoles.Operator;

    public static bool IsAllowedToCreateEntities(this LoggedInUser user)
    {
        return RolesAllowedToCreateEntities.HasFlag(user.Role);
    }
}