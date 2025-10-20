using Microsoft.Extensions.Logging;
using OshCityNewsParser.Models;
using System.Security.Cryptography;
using System.Text;

namespace OshCityNewsParser.Features.Checksum
{
    internal class SHA256ChecksumService : IChecksumService
    {
        private readonly ILogger<SHA256ChecksumService> _logger;

        internal SHA256ChecksumService(ILogger<SHA256ChecksumService> logger)
        {
            _logger = logger;
        }

        internal async Task<string> CalculateChecksumAsync(List<Article> articles)
        {
            try
            {
                _logger.LogInformation("Calculating checksum for {ArticleCount} articles", articles.Count);

                var checksums = new List<string>();

                foreach (var article in articles)
                {
                    var articleChecksum = await CalculateArticleChecksumAsync(article);
                    checksums.Add(articleChecksum);
                }

                // Concatenate all article checksums
                var concatenated = string.Concat(checksums);
                var finalChecksum = ComputeSha256(concatenated);

                _logger.LogInformation("Checksum calculated: {Checksum}", finalChecksum);
                return finalChecksum;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating checksum");
                throw;
            }
        }

        private async Task<string> CalculateArticleChecksumAsync(Article article)
        {
            return await Task.Run(() =>
            {
                var data = $"{article.SequenceNum}|{article.PublishedDate:O}|{article.Title}|{article.Text}";
                return ComputeSha256(data);
            });
        }

        private static string ComputeSha256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToHexString(hashedBytes);
            }
        }

        // IChecksumService explicit implementation
        async Task<string> IChecksumService.CalculateChecksumAsync(List<Article> articles)
            => await CalculateChecksumAsync(articles);
    }
}