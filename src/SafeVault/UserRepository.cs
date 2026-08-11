using System.Data.Common;

namespace SafeVault;

public sealed class UserRepository
{
    public DbCommand CreateFindUserByUsernameCommand(DbConnection connection, string username)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var normalizedUsername = InputSanitizer.SanitizeUsername(username);

        var command = connection.CreateCommand();
        command.CommandText = """
            SELECT UserID, Username, Email
            FROM Users
            WHERE Username = @Username
            """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@Username";
        parameter.DbType = System.Data.DbType.String;
        parameter.Value = normalizedUsername;
        command.Parameters.Add(parameter);

        return command;
    }
}
