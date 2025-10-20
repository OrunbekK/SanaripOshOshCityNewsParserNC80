using OshCityNewsParser.Models;

namespace OshCityNewsParser.Features.Checksum
{
    internal interface IChecksumService
    {
        Task<string> CalculateChecksumAsync(List<Article> articles);
    }
}