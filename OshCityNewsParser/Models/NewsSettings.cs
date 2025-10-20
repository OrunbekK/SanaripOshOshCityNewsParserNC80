namespace OshCityNewsParser.Models
{
    internal class NewsSettings
    {
        public string Setting { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public DateTime UpdatedDT { get; set; } = DateTime.Now;
    }
}