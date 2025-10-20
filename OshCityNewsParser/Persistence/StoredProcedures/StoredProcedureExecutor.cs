using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace OshCityNewsParser.Persistence.StoredProcedures
{
    internal class StoredProcedureExecutor : IStoredProcedureExecutor
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StoredProcedureExecutor> _logger;

        internal StoredProcedureExecutor(IConfiguration configuration, ILogger<StoredProcedureExecutor> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection not configured");
        }

        async Task<object?> IStoredProcedureExecutor.ExecuteScalarAsync(string spName, DynamicParameters? parameters)
        {
            try
            {
                _logger.LogInformation("Executing SP: {SpName}", spName);

                using var connection = new SqlConnection(GetConnectionString());
                var result = await connection.ExecuteScalarAsync(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("SP {SpName} executed successfully", spName);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP: {SpName}", spName);
                throw;
            }
        }

        async Task<IEnumerable<T>> IStoredProcedureExecutor.ExecuteQueryAsync<T>(string spName, DynamicParameters? parameters)
        {
            try
            {
                _logger.LogInformation("Executing SP: {SpName}", spName);

                using var connection = new SqlConnection(GetConnectionString());
                var result = await connection.QueryAsync<T>(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("SP {SpName} executed successfully, returned {Count} rows", spName, result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP: {SpName}", spName);
                throw;
            }
        }

        async Task IStoredProcedureExecutor.ExecuteAsync(string spName, DynamicParameters? parameters)
        {
            try
            {
                _logger.LogInformation("Executing SP: {SpName}", spName);

                using var connection = new SqlConnection(GetConnectionString());
                await connection.ExecuteAsync(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                _logger.LogInformation("SP {SpName} executed successfully", spName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SP: {SpName}", spName);
                throw;
            }
        }
    }
}