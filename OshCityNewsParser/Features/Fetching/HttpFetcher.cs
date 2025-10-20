using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OshCityNewsParser.Configuration;
using Polly;

namespace OshCityNewsParser.Features.Fetching
{
    internal class HttpFetcher : IHttpFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly RateLimiter _rateLimiter;
        private readonly ILogger<HttpFetcher> _logger;

        internal HttpFetcher(IOptions<ParserOptions> options, ILogger<HttpFetcher> logger)
        {
            _logger = logger;

            var parserOptions = options.Value;

            // HttpClient configuration
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", parserOptions.HttpUserAgent);
            _httpClient.Timeout = TimeSpan.FromSeconds(parserOptions.HttpTimeoutSeconds);

            // Retry policy with Polly
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: parserOptions.RetryPolicyAttempts,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromMilliseconds(parserOptions.RetryDelayMilliseconds * retryAttempt),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {DelayMs}ms", retryCount, timespan.TotalMilliseconds);
                    });

            // Rate limiter
            _rateLimiter = new RateLimiter(parserOptions.RateLimitRequestsPerSecond);
        }

        internal async Task<string> FetchHtmlAsync(string url)
        {
            _logger.LogInformation("Fetching URL: {Url}", url);

            try
            {
                await _rateLimiter.WaitIfNeededAsync();

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _httpClient.GetAsync(url));

                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Successfully fetched {Url}, size: {Size} bytes", url, html.Length);

                return html;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching URL: {Url}", url);
                throw;
            }
        }

        // IHttpFetcher explicit implementation
        async Task<string> IHttpFetcher.FetchHtmlAsync(string url)
            => await FetchHtmlAsync(url);
    }
}