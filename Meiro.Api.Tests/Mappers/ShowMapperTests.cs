using AutoFixture;
using FluentAssertions;
using Meiro.Api.Mappers;
using Meiro.Domain;

namespace Meiro.Api.Tests.Mappers;

public class ShowMapperTests
{
    [Fact]
    public void MapToContract_MapsShow()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        var showId = fixture.Create<int>();
        var name = fixture.Create<string>();

        var castIdOne = fixture.Create<int>();
        var castIdTwo = fixture.Create<int>();
        var castNameOne = fixture.Create<string>();
        var castNameTwo = fixture.Create<string>();
        var castBirthdayOne = fixture.Create<DateOnly>();
        var castBirthdayTwo = fixture.Create<DateOnly>();
        var castOne = new Cast(castIdOne, castNameOne, castBirthdayOne);
        var castTwo = new Cast(castIdTwo, castNameTwo, castBirthdayTwo);
        
        var sut = new ShowMapper();

        var result = sut.MapToContract(new Show { Id = showId, Name = name, Cast = [castOne, castTwo] });

        result.Id.Should().Be(showId);
        result.Name.Should().Be(name);
        result.Cast.Should().HaveCount(2);
        result.Cast.Should().Contain(c => c.Id == castIdOne && c.Name == castNameOne && c.Birthday == castBirthdayOne);
        result.Cast.Should().Contain(c => c.Id == castIdTwo && c.Name == castNameTwo && c.Birthday == castBirthdayTwo);
    }
}