using System.Data.Common;

namespace DatingAgencyMS.Infrastructure.Extensions;

public static class DbExtensions
{
    public static DbCommand CreateCommandWithAssignedTransaction(this DbTransaction transaction)
    {
        var cmd = transaction.Connection.CreateCommand();
        cmd.Connection = transaction.Connection;
        cmd.Transaction = transaction;
        return cmd;
    }

    /// <summary>
    /// Convenient way to run transaction without needing to worry about the connecting several objects.
    /// Lets developers concentrate mainly on the transaction logic
    /// </summary>
    /// <param name="connection">Current connection</param>
    /// <param name="transactionRunner">Logic inside the transaction</param>
    /// <param name="exceptionHandler">Custom exception handling</param>
    public static async ValueTask RunTransaction(
        this DbConnection connection,
        Action<DbTransaction, DbCommand> transactionRunner, 
        Action<DbException>? exceptionHandler = null)
    {
        DbTransaction? transaction = null;
        try
        {
            transaction = await connection.BeginTransactionAsync();
            var cmd = transaction.CreateCommandWithAssignedTransaction();
            transactionRunner(transaction, cmd);
            await transaction.CommitAsync();
        }
        catch (DbException e)
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
                await transaction.DisposeAsync();
            }
            exceptionHandler?.Invoke(e);
        }
    }
}