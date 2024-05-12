using Meiro.Application.ExternalServices;
using Meiro.Domain;

namespace Meiro.Application.Orchestrators;

public interface IGetShowsOrchestrator
{
    Task<IEnumerable<Show>> GetShows(int pageId, int pageSize, CancellationToken cancellationToken);
}

public class GetShowsOrchestrator(IShowRepository repository) : IGetShowsOrchestrator
{
    public async Task<IEnumerable<Show>> GetShows(int pageId, int pageSize, CancellationToken cancellationToken)
    {
        if (pageId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageId), "Page id should be greater or equal to one");
        }
        
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size should be greater or equal to one");
        }

        return await repository.GetShows(pageId, pageSize, cancellationToken);
    }
}