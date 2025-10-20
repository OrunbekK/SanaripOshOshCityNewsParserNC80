namespace OshCityNewsParser.Configuration
{
    internal class ParserOptions
    {
        public string NewsPageUrl { get; set; } = string.Empty;
        public int HttpTimeoutSeconds { get; set; } = 30;
        public int RateLimitRequestsPerSecond { get; set; } = 2;
        public int RetryPolicyAttempts { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 1000;
    }
}