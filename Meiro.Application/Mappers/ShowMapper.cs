using Meiro.Application.Models;

namespace Meiro.Application.Mappers;

public interface IShowMapper
{
    Domain.Show MapToDomain(Show show, Cast[] cast);
}

public class ShowMapper : IShowMapper
{
    public Domain.Show MapToDomain(Show show, Cast[] cast)
    {
        return new Domain.Show
        {
            Id = show.Id,
            Name = show.Name,
            Cast = cast.Select(c => new Domain.Cast(c.Id, c.Name, c.Birthday)).ToArray()
        };
    }
}