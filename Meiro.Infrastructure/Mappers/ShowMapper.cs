using Meiro.Application.Models;

namespace Meiro.Infrastructure.Mappers;

public interface IShowMapper
{
    Show MapToApplication(TvMaze.Contract.Show show);
}

public class ShowMapper : IShowMapper
{
    public Show MapToApplication(TvMaze.Contract.Show show)
    {
        return new Show(show.Id, show.Name);
    }
}