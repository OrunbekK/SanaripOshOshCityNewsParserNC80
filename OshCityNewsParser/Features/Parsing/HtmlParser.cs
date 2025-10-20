using AngleSharp;
using Microsoft.Extensions.Logging;
using OshCityNewsParser.Models;
using AngleSharpConfig = AngleSharp.Configuration;

namespace OshCityNewsParser.Features.Parsing
{
    internal class HtmlParser : IParsersService
    {
        private readonly SelectorsProvider _selectorsProvider;
        private readonly ILogger<HtmlParser> _logger;

        internal HtmlParser(SelectorsProvider selectorsProvider, ILogger<HtmlParser> logger)
        {
            _selectorsProvider = selectorsProvider;
            _logger = logger;
        }

        internal async Task<List<Article>> ParseArticlesAsync(string html, byte languageUid, string languageCode)
        {
            var articles = new List<Article>();

            try
            {
                _logger.LogInformation("Parsing HTML for language {LanguageCode}", languageCode);

                var selectors = _selectorsProvider.GetSelectors(languageCode);
                var context = BrowsingContext.New(AngleSharpConfig.Default);
                var document = await context.OpenAsync(req => req.Content(html));

                var articleElements = document.QuerySelectorAll(selectors.ArticleContainer);
                _logger.LogInformation("Found {Count} article elements", articleElements.Length);

                int sequenceNum = 0;
                foreach (var element in articleElements)
                {
                    try
                    {
                        var title = element.QuerySelector(selectors.ArticleTitle)?.TextContent.Trim() ?? string.Empty;
                        var url = element.QuerySelector(selectors.ArticleUrl)?.GetAttribute("href") ?? string.Empty;
                        var text = element.QuerySelector(selectors.ArticleText)?.TextContent.Trim() ?? string.Empty;
                        var dateStr = element.QuerySelector(selectors.ArticleDate)?.TextContent.Trim() ?? string.Empty;
                        var thumbnailUrl = element.QuerySelector(selectors.ArticleThumbnail)?.GetAttribute("src") ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url))
                        {
                            _logger.LogWarning("Skipping article: missing title or URL");
                            continue;
                        }

                        if (!DateTime.TryParse(dateStr, out var publishedDate))
                        {
                            publishedDate = DateTime.UtcNow;
                        }

                        var article = new Article
                        {
                            UID = Guid.NewGuid(),
                            LanguageUID = languageUid,
                            SequenceNum = ++sequenceNum,
                            PublishedDate = publishedDate,
                            Title = title,
                            Text = text,
                            Url = url,
                            ThumbnailUrl = thumbnailUrl,
                            UpdatedDT = DateTime.UtcNow
                        };

                        articles.Add(article);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error parsing individual article element");
                        continue;
                    }
                }

                _logger.LogInformation("Parsed {Count} articles successfully", articles.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing HTML for language {LanguageCode}", languageCode);
                throw;
            }

            return articles;
        }

        // IParsersService explicit implementation
        async Task<List<Article>> IParsersService.ParseArticlesAsync(string html, byte languageUid, string languageCode)
            => await ParseArticlesAsync(html, languageUid, languageCode);
    }
}