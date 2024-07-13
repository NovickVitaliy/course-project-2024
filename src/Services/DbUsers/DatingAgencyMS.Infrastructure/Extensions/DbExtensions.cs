using System.Data.Common;

namespace DatingAgencyMS.Infrastructure.Extensions;

public static class DbExtensions
{
    public static DbCommand CreateCommandWithAssignedTransaction(this DbTransaction transaction)
    {
        var cmd = transaction.Connection.CreateCommand();
        cmd.Transaction = transaction;
        return cmd;
    }
}