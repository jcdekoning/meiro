using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Meiro.Infrastructure.RateLimiter;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly IRateLimiter _rateLimiter;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public RateLimitingHandler(IRateLimiter rateLimiter, ILogger<RateLimitingHandler> logger)
    {
        _rateLimiter = rateLimiter;
        _retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 10,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    _rateLimiter.DecreaseRequests();
                    logger.LogInformation(
                        "Waiting {timespan} seconds before retry #{retryAttempt}. HTTP Status code: {statusCode}",
                        timespan.TotalSeconds, retryAttempt, outcome.Result.StatusCode);
                    
                });
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            await _rateLimiter.WaitForNextRequestSlotAsync();
            return await base.SendAsync(request, cancellationToken);
        });
    }
}