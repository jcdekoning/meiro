namespace Meiro.Infrastructure.RateLimiter;

public interface IRateLimiter
{
    Task WaitForNextRequestSlotAsync();
    void DecreaseRequests();
}