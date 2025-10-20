using Microsoft.Extensions.Logging;
using OshCityNewsParser.Features.Checksum;
using OshCityNewsParser.Features.Fetching;
using OshCityNewsParser.Features.Normalization;
using OshCityNewsParser.Features.Parsing;
using OshCityNewsParser.Persistence;

namespace OshCityNewsParser.Features.Processing
{
    internal class NewsProcessingService : INewsProcessingService
    {
        private readonly IHttpFetcher _httpFetcher;
        private readonly IParsersService _parsersService;
        private readonly IChecksumService _checksumService;
        private readonly INormalizeService _normalizeService;
        private readonly INewsRepository _newsRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILogger<NewsProcessingService> _logger;

        internal NewsProcessingService(
            IHttpFetcher httpFetcher,
            IParsersService parsersService,
            IChecksumService checksumService,
            INormalizeService normalizeService,
            INewsRepository newsRepository,
            ISettingsRepository settingsRepository,
            ILogger<NewsProcessingService> logger)
        {
            _httpFetcher = httpFetcher;
            _parsersService = parsersService;
            _checksumService = checksumService;
            _normalizeService = normalizeService;
            _newsRepository = newsRepository;
            _settingsRepository = settingsRepository;
            _logger = logger;
        }

        internal async Task ProcessLanguageAsync(string languageCode, byte languageUid, string newsUrl)
        {
            try
            {
                _logger.LogInformation("Starting processing for language {LanguageCode}", languageCode);

                // Step 1: Fetch HTML
                _logger.LogInformation("Step 1: Fetching HTML from {Url}", newsUrl);
                var html = await _httpFetcher.FetchHtmlAsync(newsUrl);

                // Step 2: Parse articles
                _logger.LogInformation("Step 2: Parsing articles");
                var articles = await _parsersService.ParseArticlesAsync(html, languageUid, languageCode);
                _logger.LogInformation("Parsed {Count} articles", articles.Count);

                if (articles.Count == 0)
                {
                    _logger.LogWarning("No articles parsed for language {LanguageCode}", languageCode);
                    return;
                }

                // Step 3: Normalize text
                _logger.LogInformation("Step 3: Normalizing article text");
                foreach (var article in articles)
                {
                    article.Title = _normalizeService.NormalizeText(article.Title);
                    article.Text = _normalizeService.NormalizeText(article.Text);
                }

                // Step 4: Calculate checksum
                _logger.LogInformation("Step 4: Calculating checksum");
                var checksum = await _checksumService.CalculateChecksumAsync(articles);
                _logger.LogInformation("Checksum calculated: {Checksum}", checksum);

                // Step 5: Upsert articles to database
                _logger.LogInformation("Step 5: Upserting articles to database");
                foreach (var article in articles)
                {
                    article.CheckSum = checksum;
                    var (result, message) = await _newsRepository.UpsertArticleAsync(article);

                    if (result != 0)
                    {
                        _logger.LogWarning("Upsert article failed: {Message}", message);
                    }
                }

                // Step 6: Update checksum in settings
                _logger.LogInformation("Step 6: Updating checksum in settings");
                var (updateResult, updateMessage) = await _settingsRepository.UpdateChecksumAsync(languageUid, checksum);

                if (updateResult == 0)
                {
                    _logger.LogInformation("Checksum updated successfully for language {LanguageCode}", languageCode);
                }
                else
                {
                    _logger.LogWarning("Checksum update failed: {Message}", updateMessage);
                }

                _logger.LogInformation("Processing completed successfully for language {LanguageCode}", languageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing language {LanguageCode}", languageCode);
                throw;
            }
        }

        // INewsProcessingService explicit implementation
        async Task INewsProcessingService.ProcessLanguageAsync(string languageCode, byte languageUid, string newsUrl)
            => await ProcessLanguageAsync(languageCode, languageUid, newsUrl);
    }
}