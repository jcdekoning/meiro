using Meiro.Application.ExternalServices;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Meiro.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string mongoDbConnectionString,
        string mongoDbDatabase)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(mongoDbConnectionString));

        services.AddTransient<IShowRepository, ShowRepository>(sp =>
            new ShowRepository(sp.GetRequiredService<IMongoClient>(), mongoDbDatabase));

        return services;
    }
}