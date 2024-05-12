using Meiro.Application.ExternalServices;
using Meiro.Infrastructure.Mappers;
using Meiro.Infrastructure.RateLimiter;
using Meiro.Infrastructure.TvMaze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;

namespace Meiro.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Uri tvMazeUrl)
    {
        // services.AddSingleton<IRateLimiter, FixedIntervalRateLimiter>(sp new FixedIntervalRateLimiter(120));
        services.AddSingleton<IRateLimiter, DynamicIntervalRateLimiter>(sp =>
            new DynamicIntervalRateLimiter(120, 6000, sp.GetRequiredService<ILogger<DynamicIntervalRateLimiter>>()));
        services.AddRefitClient<ITvMazeClient>()
            .ConfigureHttpClient(c => c.BaseAddress = tvMazeUrl)
            .AddHttpMessageHandler(provider =>
                new RateLimitingHandler(provider.GetRequiredService<IRateLimiter>(),
                    provider.GetRequiredService<ILogger<RateLimitingHandler>>()));
        
        services.AddTransient<ITvInformationClient, TvInformationClient>();

        services.AddSingleton<IShowMapper, ShowMapper>();
        services.AddSingleton<ICastMapper, CastMapper>();
        
        return services;
    }
}