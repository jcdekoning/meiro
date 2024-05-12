using Meiro.Api.Mappers;
using Meiro.Application.Orchestrators;

namespace Meiro.Api.Handlers;

public static class ShowHandler
{
    public static async Task<IResult> GetShows(IGetShowsOrchestrator getShowsOrchestrator, IShowMapper mapper,
        HttpContext httpContext, int page = 1, int pageSize = 10)
    {
        try
        {
            var shows = await getShowsOrchestrator.GetShows(page, pageSize, httpContext.RequestAborted);
            return Results.Ok(shows.Select(mapper.MapToContract).ToList());
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}