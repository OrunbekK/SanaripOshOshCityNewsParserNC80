using System.Diagnostics;

namespace OshCityNewsParser.Features.Fetching
{
    internal class RateLimiter
    {
        private readonly int _requestsPerSecond;
        private readonly Queue<long> _requestTimestamps;
        private readonly object _lockObject = new();

        internal RateLimiter(int requestsPerSecond)
        {
            _requestsPerSecond = requestsPerSecond;
            _requestTimestamps = new Queue<long>(requestsPerSecond);
        }

        internal async Task WaitIfNeededAsync()
        {
            lock (_lockObject)
            {
                var now = Stopwatch.GetTimestamp();
                var oneSecondAgo = now - (Stopwatch.Frequency);

                while (_requestTimestamps.Count > 0 && _requestTimestamps.Peek() < oneSecondAgo)
                {
                    _requestTimestamps.Dequeue();
                }

                if (_requestTimestamps.Count >= _requestsPerSecond)
                {
                    var oldestRequest = _requestTimestamps.Peek();
                    var waitTime = (oldestRequest + Stopwatch.Frequency - now) / (double)Stopwatch.Frequency;
                    return;
                }

                _requestTimestamps.Enqueue(now);
            }

            await Task.CompletedTask;
        }
    }
}