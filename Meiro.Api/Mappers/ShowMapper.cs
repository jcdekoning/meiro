using Meiro.Domain;

namespace Meiro.Api.Mappers;

public interface IShowMapper
{
    Contract.Show MapToContract(Show show);
}

public class ShowMapper : IShowMapper
{
    public Contract.Show MapToContract(Show show)
    {
        return new Contract.Show(show.Id, show.Name,
            show.Cast.Select(c => new Contract.Cast(c.Id, c.Name, c.Birthday)).ToArray());
    }
}