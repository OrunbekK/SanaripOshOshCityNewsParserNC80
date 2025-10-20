using OshCityNewsParser.Models;

namespace OshCityNewsParser.Persistence
{
    internal interface INewsRepository
    {
        Task<(int Result, string Message)> UpsertArticleAsync(Article article);
    }
}