using Dapper;
using Npgsql;

namespace Template.Jobs.Data;

internal class DatabaseProvider(NpgsqlDataSource dataSource)
{
    public async Task<T> GetFirstOrDefault<T>(string sql, object obj, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = dataSource.CreateConnection();
            var command = new CommandDefinition(sql, parameters: obj, cancellationToken: cancellationToken);
            return await connection.QueryFirstOrDefaultAsync<T>(command) ?? default!;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Fail on execute query.", ex);
        }
    }

    public async Task<T> GetFirstOrDefault<T>(string sql, CancellationToken cancellationToken)
    {
        return await GetFirstOrDefault<T>(sql, null!, cancellationToken);
    }
}

public class DatabaseException(string message, Exception innerException) : Exception(message, innerException);