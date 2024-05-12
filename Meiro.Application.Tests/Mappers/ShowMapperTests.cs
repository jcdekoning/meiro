using AutoFixture;
using FluentAssertions;
using Meiro.Application.Mappers;

namespace Meiro.Application.Tests.Mappers;

public class ShowMapperTests
{
    [Fact]
    public void MapToDomain_MapsShow()
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
        var castOne = new Models.Cast(castIdOne, castNameOne, castBirthdayOne);
        var castTwo = new Models.Cast(castIdTwo, castNameTwo, castBirthdayTwo);
        
        var sut = new ShowMapper();

        var result = sut.MapToDomain(new Application.Models.Show(showId, name), [castOne, castTwo]);

        result.Id.Should().Be(showId);
        result.Name.Should().Be(name);
        result.Cast.Should().HaveCount(2);
        result.Cast.Should().Contain(c => c.Id == castIdOne && c.Name == castNameOne && c.Birthday == castBirthdayOne);
        result.Cast.Should().Contain(c => c.Id == castIdTwo && c.Name == castNameTwo && c.Birthday == castBirthdayTwo);
    }
}