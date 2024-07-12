namespace DatingAgencyMS.Client.Models.Core;

[Flags]
public enum DbRoles
{
    None = 0,
    Owner = 1,
    Admin = 2,
    Operator = 4,
    Guest = 8
}