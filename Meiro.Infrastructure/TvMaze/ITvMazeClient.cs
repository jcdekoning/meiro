using Meiro.Infrastructure.TvMaze.Contract;
using Refit;

namespace Meiro.Infrastructure.TvMaze;

public interface ITvMazeClient
{
    [Get("/shows?page={page}")]
    Task<Show[]> GetShows(int page, CancellationToken cancellationToken);

    [Get("/shows/{showId}/cast")]
    Task<Cast[]> GetShowCast(int showId, CancellationToken cancellationToken);
}