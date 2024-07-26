namespace DatingAgencyMS.Domain.Models.DbManagement;

[Flags]
public enum DbRoles
{
    Owner = 1,
    Admin = 2,
    Operator = 4,
    Guest = 8
}