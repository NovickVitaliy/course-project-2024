using System.Data.Common;

namespace DatingAgencyMS.Infrastructure.Helpers;

public static class DbHelper
{
    public static DbCommand AddParameter(this DbCommand cmd, string parameterName, object? value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = parameterName;
        param.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(param);
        return cmd;
    }
}