namespace DatingAgencyMS.Domain.Models;

public class DbUser
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public DbRole DbRole { get; set; }
    public int DbRoleId { get; set; }
}