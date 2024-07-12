namespace DatingAgencyMS.Domain.Models;

public class DbUser
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DbRoles Role { get; set; }
}