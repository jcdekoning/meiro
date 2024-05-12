using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Meiro.Infrastructure.RateLimiter;

public class DynamicIntervalRateLimiter : IRateLimiter
{
    private TimeSpan _requestInterval;
    private readonly int _minimumRequestPerMinute;
    private readonly int _maximumRequestPerMinute;
    private readonly ILogger<DynamicIntervalRateLimiter> _logger;
    private double _currentRequestPerMinute;
    private DateTime _lastRequestTime = DateTime.MinValue;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Timer _increaseTimer;
    private readonly object _timerLock = new();
    
    public DynamicIntervalRateLimiter(int minimumRequestPerMinute, int maximumRequestPerMinute, ILogger<DynamicIntervalRateLimiter> logger)
    {
        _minimumRequestPerMinute = minimumRequestPerMinute;
        _maximumRequestPerMinute = maximumRequestPerMinute;
        _logger = logger;
        _currentRequestPerMinute = maximumRequestPerMinute;
        _requestInterval = CalculateInterval(_currentRequestPerMinute);
        _increaseTimer = new Timer(IncreaseRequests, null, Timeout.Infinite, Timeout.Infinite);
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
        lock (_timerLock)
        {
            var reduction = _maximumRequestPerMinute * 0.9; //todo make configurable
            _currentRequestPerMinute = Math.Max(_currentRequestPerMinute - reduction, _minimumRequestPerMinute);
            _logger.LogInformation("Decreased number of request per minute to {requestsPerMinute}", _currentRequestPerMinute);
            _requestInterval = CalculateInterval(_currentRequestPerMinute);
            _increaseTimer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan); //make configurable
        }
    }
    
    private void IncreaseRequests(object? state)
    {
        lock (_timerLock)
        {
            if (_currentRequestPerMinute < _maximumRequestPerMinute)
            {
                var increase = _maximumRequestPerMinute * 0.05; //make configurable
                _currentRequestPerMinute = Math.Min(_currentRequestPerMinute + increase, _maximumRequestPerMinute);
                _logger.LogInformation("Increased number of request per minute to {requestsPerMinute}", _currentRequestPerMinute);
                _requestInterval = CalculateInterval(_currentRequestPerMinute);
            }
            if (_currentRequestPerMinute >= _maximumRequestPerMinute)
            {
                _increaseTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
    }
    
    private static TimeSpan CalculateInterval(double requestPerMinute)
    {
        var intervalInMilliseconds = 60000.0 / requestPerMinute;
        return  TimeSpan.FromMilliseconds(intervalInMilliseconds);
    }
}