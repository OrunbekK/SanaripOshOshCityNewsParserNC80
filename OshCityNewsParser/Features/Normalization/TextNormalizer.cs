using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace OshCityNewsParser.Features.Normalization
{
    internal class TextNormalizer : INormalizeService
    {
        private readonly ILogger<TextNormalizer> _logger;

        internal TextNormalizer(ILogger<TextNormalizer> logger)
        {
            _logger = logger;
        }

        internal string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            try
            {
                // Remove extra whitespace (multiple spaces, tabs, newlines)
                text = Regex.Replace(text, @"\s+", " ");

                // Trim leading and trailing whitespace
                text = text.Trim();

                // Remove HTML entities if any
                text = System.Net.WebUtility.HtmlDecode(text);

                // Remove control characters
                text = Regex.Replace(text, @"[\x00-\x1F\x7F]", string.Empty);

                _logger.LogDebug("Text normalized successfully");
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error normalizing text");
                throw;
            }
        }

        // INormalizeService explicit implementation
        string INormalizeService.NormalizeText(string text)
            => NormalizeText(text);
    }
}