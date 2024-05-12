using AutoFixture;
using FluentAssertions;
using Meiro.Infrastructure.Mappers;
using Meiro.Infrastructure.TvMaze.Contract;

namespace Meiro.Infrastructure.Tests.Mappers;

public class CastMapperTests
{
    [Fact]
    public void MapToApplication_MapsCast()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        var castId = fixture.Create<int>();
        var name = fixture.Create<string>();
        var birthday = fixture.Create<DateOnly>();

        var sut = new CastMapper();

        var result = sut.MapToApplication(new Cast(new Person(castId, name, birthday)));

        result.Id.Should().Be(castId);
        result.Name.Should().Be(name);
        result.Birthday.Should().Be(birthday);
    }
}