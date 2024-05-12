using Meiro.Application.ExternalServices;
using Meiro.Application.Mappers;
using Microsoft.Extensions.Logging;

namespace Meiro.Application.Orchestrators;

public interface IImportShowsOrchestrator
{
    Task Import(CancellationToken cancellationToken);
}

public class ImportShowsOrchestrator(
    ITvInformationClient tvInformationClient,
    IShowRepository showRepository,
    IShowMapper mapper,
    ILogger<ImportShowsOrchestrator> logger) : IImportShowsOrchestrator
{
    public async Task Import(CancellationToken cancellationToken)
    {
        var page = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation("Get shows for page {page}", page);
            var shows = await tvInformationClient.GetShows(page, cancellationToken);
            logger.LogInformation("Retrieved {numberOfShows} shows for page {page}", shows.Length, page);
            if (shows.Length == 0)
            {
                logger.LogInformation("No shows retrieved. Done!");
                return;
            }

            foreach (var show in shows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                logger.LogInformation("Get cast for show {showId} {showName}", show.Id, show.Name);
                var cast = await tvInformationClient.GetCastForShow(show.Id, cancellationToken);
                logger.LogInformation("Retrieved cast of {castLength} for show {showId} {showName}", cast.Length, show.Id, show.Name);
                var domainShow = mapper.MapToDomain(show, cast.OrderByDescending(c => c.Birthday).ToArray());
                
                await showRepository.AddOrUpdateShow(domainShow, cancellationToken);
                logger.LogInformation("Show {showId} {showName} stored", show.Id, show.Name);
            }

            page++;
        }
    }
}