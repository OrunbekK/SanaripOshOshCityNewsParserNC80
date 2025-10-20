namespace OshCityNewsParser.Configuration
{
    public sealed class LoggingOptions
    {
        public string MinimumLevel { get; set; } = "Information";
        public List<string> Using { get; set; } = new();
        public Dictionary<string, object> WriteTo { get; set; } = new();
    }
}