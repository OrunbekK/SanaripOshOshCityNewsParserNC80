namespace OshCityNewsParser.Features.Processing
{
    internal interface INewsProcessingService
    {
        Task ProcessLanguageAsync(string languageCode, byte languageUid, string newsUrl);
    }
}