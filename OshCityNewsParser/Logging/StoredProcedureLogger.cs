using Microsoft.Extensions.Logging;

namespace OshCityNewsParser.Logging
{
    internal static class StoredProcedureLogger
    {
        internal static void LogSpExecution(ILogger logger, string spName, int? result = null, string? message = null)
        {
            if (result == 0)
                logger.LogInformation("SP {SpName} executed successfully", spName);
            else
                logger.LogWarning("SP {SpName} returned result: {Result}, Message: {Message}", spName, result, message);
        }

        internal static void LogSpError(ILogger logger, string spName, Exception ex)
        {
            logger.LogError(ex, "Error executing SP: {SpName}", spName);
        }

        internal static void LogSpStart(ILogger logger, string spName)
        {
            logger.LogDebug("Starting SP execution: {SpName}", spName);
        }
    }
}