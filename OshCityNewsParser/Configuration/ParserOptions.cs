namespace OshCityNewsParser.Configuration
{
    internal class ParserOptions
    {
        public List<LanguageConfig> Languages { get; set; } = new();
        public string HttpUserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
        public int HttpTimeoutSeconds { get; set; } = 30;
        public int RateLimitRequestsPerSecond { get; set; } = 2;
        public int RetryPolicyAttempts { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 1000;
    }

    internal class LanguageConfig
    {
        public string Code { get; set; } = string.Empty;
        public string NewsPageUrl { get; set; } = string.Empty;
        public byte LanguageUID { get; set; }
    }
}