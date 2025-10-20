using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OshCityNewsParser.Configuration;
using YamlDotNet.Serialization;
using AngleSharpConfig = AngleSharp.Configuration;

namespace OshCityNewsParser.Features.Parsing
{
    internal class SelectorsProvider
    {
        private readonly Dictionary<string, Selectors> _selectorsByLanguage;
        private readonly ILogger<SelectorsProvider> _logger;

        internal SelectorsProvider(IOptions<ParserOptions> options, ILogger<SelectorsProvider> logger)
        {
            _logger = logger;
            _selectorsByLanguage = new Dictionary<string, Selectors>();

            var parserOptions = options.Value;
            LoadSelectorsAsync(parserOptions).GetAwaiter().GetResult();
        }

        private async Task LoadSelectorsAsync(ParserOptions options)
        {
            try
            {
                foreach (var language in options.Languages)
                {
                    await LoadLanguageSelectorsAsync(language.Code, language.SelectorsPath);
                }

                _logger.LogInformation("Selectors loaded for languages: {Languages}", string.Join(", ", _selectorsByLanguage.Keys));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading selectors");
                throw;
            }
        }

        private async Task LoadLanguageSelectorsAsync(string languageCode, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Selectors file not found: {filePath}");
                }

                var yaml = await File.ReadAllTextAsync(filePath);
                var deserializer = new DeserializerBuilder().Build();
                var selectors = deserializer.Deserialize<Selectors>(yaml);

                _selectorsByLanguage[languageCode] = selectors;
                _logger.LogInformation("Loaded selectors for language: {LanguageCode} from {FilePath}", languageCode, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading selectors for language {LanguageCode} from {FilePath}", languageCode, filePath);
                throw;
            }
        }

        internal Selectors GetSelectors(string languageCode)
        {
            if (!_selectorsByLanguage.TryGetValue(languageCode, out var selectors))
            {
                _logger.LogWarning("Selectors not found for language: {LanguageCode}", languageCode);
                throw new InvalidOperationException($"Selectors not found for language: {languageCode}");
            }

            return selectors;
        }
    }

    internal sealed class Selectors
    {
        [YamlMember(Alias = "article_container")]
        public string ArticleContainer { get; set; } = string.Empty;

        [YamlMember(Alias = "article_title")]
        public string ArticleTitle { get; set; } = string.Empty;

        [YamlMember(Alias = "article_url")]
        public string ArticleUrl { get; set; } = string.Empty;

        [YamlMember(Alias = "article_text")]
        public string ArticleText { get; set; } = string.Empty;

        [YamlMember(Alias = "article_date")]
        public string ArticleDate { get; set; } = string.Empty;

        [YamlMember(Alias = "article_thumbnail")]
        public string ArticleThumbnail { get; set; } = string.Empty;

        [YamlMember(Alias = "article_sequence_num")]
        public string ArticleSequenceNum { get; set; } = string.Empty;
    }
}