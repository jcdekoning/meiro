using Meiro.Application.Mappers;
using Meiro.Application.Orchestrators;
using Microsoft.Extensions.DependencyInjection;

namespace Meiro.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IImportShowsOrchestrator, ImportShowsOrchestrator>();
        services.AddTransient<IGetShowsOrchestrator, GetShowsOrchestrator>();

        services.AddSingleton<IShowMapper, ShowMapper>();
        
        return services;
    }
}