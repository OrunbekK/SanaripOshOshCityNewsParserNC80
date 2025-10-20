using OshCityNewsParser.Models;

namespace OshCityNewsParser.Persistence
{
    internal interface ISettingsRepository
    {
        Task<(int Result, string Message)> UpdateChecksumAsync(byte languageUid, string checksum);
    }
}