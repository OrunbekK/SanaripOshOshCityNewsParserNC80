namespace OshCityNewsParser.Features.Fetching
{
    internal interface IHttpFetcher
    {
        Task<string> FetchHtmlAsync(string url);
    }
}