namespace DatingAgencyMS.Client.Models.Core;

public class User
{
    public string Login { get; set; }
    public string Token { get; set; }
    public DbRoles Role { get; set; }
}