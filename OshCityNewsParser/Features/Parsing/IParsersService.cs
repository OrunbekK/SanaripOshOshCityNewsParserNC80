using OshCityNewsParser.Models;

namespace OshCityNewsParser.Features.Parsing
{
    internal interface IParsersService
    {
        Task<List<Article>> ParseArticlesAsync(string html, byte languageUid, string languageCode);
    }
}