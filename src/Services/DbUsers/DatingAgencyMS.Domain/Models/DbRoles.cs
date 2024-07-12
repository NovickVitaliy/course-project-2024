namespace DatingAgencyMS.Domain.Models;

[Flags]
public enum DbRoles
{
    Owner = 1,
    Admin = 2,
    Operator = 4,
    Guest = 8
}