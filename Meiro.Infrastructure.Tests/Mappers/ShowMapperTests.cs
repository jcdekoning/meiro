using AutoFixture;
using FluentAssertions;
using Meiro.Infrastructure.Mappers;
using Meiro.Infrastructure.TvMaze.Contract;

namespace Meiro.Infrastructure.Tests.Mappers;

public class ShowMapperTests
{
    [Fact]
    public void MapToApplication_MapsShow()
    {
        var fixture = new Fixture();
        var showId = fixture.Create<int>();
        var name = fixture.Create<string>();

        var sut = new ShowMapper();

        var result = sut.MapToApplication(new Show(showId, name));

        result.Id.Should().Be(showId);
        result.Name.Should().Be(name);
    }
}