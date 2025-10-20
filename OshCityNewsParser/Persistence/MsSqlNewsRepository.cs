using Microsoft.Extensions.Logging;
using OshCityNewsParser.Models;
using OshCityNewsParser.Persistence.StoredProcedures;

namespace OshCityNewsParser.Persistence
{
    internal class MsSqlNewsRepository : INewsRepository
    {
        private readonly IStoredProcedureExecutor _spExecutor;
        private readonly ILogger<MsSqlNewsRepository> _logger;

        internal MsSqlNewsRepository(IStoredProcedureExecutor spExecutor, ILogger<MsSqlNewsRepository> logger)
        {
            _spExecutor = spExecutor;
            _logger = logger;
        }

        internal async Task<(int Result, string Message)> UpsertArticleAsync(Article article)
        {
            _logger.LogInformation("Upserting article: {Title} (Language: {LanguageUID})", article.Title, article.LanguageUID);

            var parameters = SpParameterBuilder.BuildUpsertArticleParameters(article);

            try
            {
                await _spExecutor.ExecuteAsync(SpNames.USP_UPSERT_ARTICLE, parameters);

                int result = parameters.Get<int>("@Result");
                string message = parameters.Get<string>("@Message") ?? string.Empty;

                if (result == 0)
                    _logger.LogInformation("Article upserted successfully: {Title}", article.Title);
                else
                    _logger.LogWarning("Article upsert returned result: {Result}, Message: {Message}", result, message);

                return (result, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting article: {Title}", article.Title);
                throw;
            }
        }

        // INewsRepository explicit implementations
        async Task<(int, string)> INewsRepository.UpsertArticleAsync(Article article)
            => await UpsertArticleAsync(article);
    }
}