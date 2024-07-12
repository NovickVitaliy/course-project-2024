namespace DatingAgencyMS.Client.Models.Core;

public class LoggedInUser
{
    public string Login { get; set; }
    public string Token { get; set; }
    public DbRoles Role { get; set; }
}