using Meiro.Application.Models;

namespace Meiro.Infrastructure.Mappers;

public interface ICastMapper
{
    Cast MapToApplication(TvMaze.Contract.Cast cast);
}

public class CastMapper : ICastMapper
{
    public Cast MapToApplication(TvMaze.Contract.Cast cast)
    {
        return new Cast(cast.Person.Id, cast.Person.Name, cast.Person.Birthday);
    }
}