using Dapper;
using Microsoft.Extensions.Logging;
using OshCityNewsParser.Persistence.StoredProcedures;

namespace OshCityNewsParser.Persistence
{
    internal class MsSqlSettingsRepository : ISettingsRepository
    {
        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<MsSqlSettingsRepository> _logger;

        internal MsSqlSettingsRepository(IStoredProcedureExecutor spExecutor, ILogger<MsSqlSettingsRepository> logger)
        {
            _spExecutor = spExecutor;
            _logger = logger;
        }

        internal async Task<(int Result, string Message)> UpdateChecksumAsync(byte languageUid, string checksum)
        {
            _logger.LogInformation("Updating checksum for language: {LanguageUID}", languageUid);

            var parameters = SpParameterBuilder.BuildUpdateChecksumParameters(languageUid, checksum);

            try
            {
                await _spExecutor.ExecuteAsync(SpNames.USP_UPDATE_CHECKSUM, parameters);

                int result = parameters.Get<int>("@Result");
                string message = parameters.Get<string>("@Message") ?? string.Empty;

                if (result == 0)
                    _logger.LogInformation("Checksum updated for language {LanguageUID}", languageUid);
                else
                    _logger.LogWarning("UpdateChecksum returned result: {Result}, Message: {Message}", result, message);

                return (result, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating checksum for language: {LanguageUID}", languageUid);
                throw;
            }
        }

        // ISettingsRepository explicit implementations
        async Task<(int, string)> ISettingsRepository.UpdateChecksumAsync(byte languageUid, string checksum)
            => await UpdateChecksumAsync(languageUid, checksum);
    }
}