using System.Data.Common;

namespace DatingAgencyMS.Infrastructure.Models;

public class DbConnectionInfo
{
    public DbConnection Connection { get; }
    public DateTime LastAccessed { get; set; }
    public bool CurrentlyInUse { get; set; }

    public DbConnectionInfo(DbConnection connection)
    {
        Connection = connection;
        LastAccessed = DateTime.Now;
        CurrentlyInUse = false;
    }
}