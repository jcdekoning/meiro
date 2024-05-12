using Meiro.Api.Mappers;

namespace Meiro.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddSingleton<IShowMapper, ShowMapper>();
        
        return services;
    }
}