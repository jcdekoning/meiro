using Meiro.Application.ExternalServices;
using Meiro.Domain;
using MongoDB.Driver;

namespace Meiro.Persistence;

public class ShowRepository : IShowRepository
{
    private readonly IMongoCollection<Show> _shows;

    public ShowRepository(IMongoClient mongoClient, string mongoDbDatabase)
    {
        var database = mongoClient.GetDatabase(mongoDbDatabase);
        _shows = database.GetCollection<Show>("Shows");
    }

    public async Task<IEnumerable<Show>> GetShows(int pageId, int pageSize, CancellationToken cancellationToken)
    {
        return (await _shows
            .Find(show => true)
            .SortBy(show => show.Id)
            .Skip((pageId - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken));
    }

    public Task AddOrUpdateShow(Show show, CancellationToken cancellationToken)
    {
        var filter = Builders<Show>.Filter.Eq(s => s.Id, show.Id);
        return _shows.ReplaceOneAsync(filter, show, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }
}