using Dapper;

namespace OshCityNewsParser.Persistence.StoredProcedures
{
    internal interface IStoredProcedureExecutor
    {
        Task<object?> ExecuteScalarAsync(string spName, DynamicParameters? parameters = null);
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string spName, DynamicParameters? parameters = null) where T : class;
        Task ExecuteAsync(string spName, DynamicParameters? parameters = null);
    }
}