using System.Net;
using Meiro.Application.ExternalServices;
using Meiro.Application.Models;
using Meiro.Infrastructure.Mappers;
using Meiro.Infrastructure.TvMaze;
using Refit;

namespace Meiro.Infrastructure;

public class TvInformationClient(ITvMazeClient tvMazeClient, IShowMapper showMapper, ICastMapper castMapper) : ITvInformationClient
{
    public async Task<Show[]> GetShows(int pageId, CancellationToken cancellationToken)
    {
        try
        {
            var shows = await tvMazeClient.GetShows(pageId, cancellationToken);
            return shows.Select(showMapper.MapToApplication).ToArray();
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }
    }

    public async Task<Cast[]> GetCastForShow(int showId, CancellationToken cancellationToken)
    {
        var cast = await tvMazeClient.GetShowCast(showId, cancellationToken);
        return cast.Select(castMapper.MapToApplication).ToArray();
    }
}