using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OshCityNewsParser.Configuration;
using OshCityNewsParser.Features.Processing;

namespace OshCityNewsParser.Workers
{
    internal class NewsParserWorker : BackgroundService
    {
        private readonly INewsProcessingService _processingService;
        private readonly IOptions<ParserOptions> _options;
        private readonly ILogger<NewsParserWorker> _logger;

        internal NewsParserWorker(
            INewsProcessingService processingService,
            IOptions<ParserOptions> options,
            ILogger<NewsParserWorker> logger)
        {
            _processingService = processingService;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NewsParserWorker started");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Starting news parsing cycle");

                        var parserOptions = _options.Value;

                        foreach (var language in parserOptions.Languages)
                        {
                            if (stoppingToken.IsCancellationRequested)
                                break;

                            _logger.LogInformation("Processing language: {LanguageCode}", language.Code);

                            await _processingService.ProcessLanguageAsync(
                                language.Code,
                                language.LanguageUID,
                                language.NewsPageUrl);

                            _logger.LogInformation("Completed processing for language: {LanguageCode}", language.Code);
                        }

                        _logger.LogInformation("News parsing cycle completed. Waiting for next cycle...");

                        // Wait 1 hour before next cycle (configurable if needed)
                        await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("NewsParserWorker cancellation requested");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in news parsing cycle");
                        // Wait before retry on error
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error in NewsParserWorker");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NewsParserWorker is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}