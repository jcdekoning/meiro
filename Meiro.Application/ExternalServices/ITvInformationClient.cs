using Meiro.Application.Models;

namespace Meiro.Application.ExternalServices;

public interface ITvInformationClient
{
    Task<Show[]> GetShows(int pageId, CancellationToken cancellationToken);

    Task<Cast[]> GetCastForShow(int showId, CancellationToken cancellationToken);
}