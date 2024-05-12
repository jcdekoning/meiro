using Meiro.Domain;

namespace Meiro.Application.ExternalServices;

public interface IShowRepository
{
    Task<IEnumerable<Show>> GetShows(int pageId, int pageSize, CancellationToken cancellationToken);
    
    Task AddOrUpdateShow(Show show, CancellationToken cancellationToken);
}