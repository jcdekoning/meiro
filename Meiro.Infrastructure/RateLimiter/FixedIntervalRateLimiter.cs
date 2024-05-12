namespace Meiro.Infrastructure.RateLimiter;

public class FixedIntervalRateLimiter : IRateLimiter
{
    private readonly TimeSpan _requestInterval;
    private DateTime _lastRequestTime = DateTime.MinValue;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public FixedIntervalRateLimiter(int requestsPerMinute)
    {
        var intervalInMilliseconds = 60000.0 / requestsPerMinute;
        _requestInterval = TimeSpan.FromMilliseconds(intervalInMilliseconds);
    }
    
    public async Task WaitForNextRequestSlotAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var now = DateTime.UtcNow; //todo inject clock
            var timeSinceLastRequest = now - _lastRequestTime;
            if (timeSinceLastRequest < _requestInterval)
            {
                var delay = _requestInterval - timeSinceLastRequest;
                await Task.Delay(delay);
            }
            _lastRequestTime = DateTime.UtcNow; //todo inject clock
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void DecreaseRequests()
    {
        // Do nothing;
    }
}