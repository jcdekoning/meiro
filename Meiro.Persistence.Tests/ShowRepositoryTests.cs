using System.Collections;
using AutoFixture;
using FluentAssertions;
using Meiro.Domain;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Meiro.Persistence.Tests;

public class ShowRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();
    
    [Fact]
    public async Task AddOrUpdateShow_AddsShowWhenItDoesNotExits()
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
        
        var client = new MongoClient(_mongoDbContainer.GetConnectionString());
        var sut = new ShowRepository(client, "TestDb");
        
        await sut.AddOrUpdateShow(new Show
        {
            Id = showId,
            Name = name,
            Cast = [castOne, castTwo]
        }, CancellationToken.None);

        var shows = (await sut.GetShows(1, 10, CancellationToken.None)).ToArray();
        
        shows.Should().HaveCount(1);
        var show = shows.First();
        show.Id.Should().Be(showId);
        show.Name.Should().Be(name);
        show.Cast.Should().HaveCount(2);
        show.Cast.Should().Contain(c => c.Id == castIdOne && c.Name == castNameOne && c.Birthday == castBirthdayOne);
        show.Cast.Should().Contain(c => c.Id == castIdTwo && c.Name == castNameTwo && c.Birthday == castBirthdayTwo);
    }
    
    [Fact]
    public async Task GetShows_ReturnsShows_ForCorrectPageAndSize()
    {
        var client = new MongoClient(_mongoDbContainer.GetConnectionString());
        var sut = new ShowRepository(client, "TestDb");
        
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

        var shows = Enumerable.Range(1, 10).Select(x => fixture.Build<Show>().With(s => s.Id, x).Create()).ToArray();
        foreach (var show in shows)
        {
            await sut.AddOrUpdateShow(show, CancellationToken.None);
        }
        
        var result = (await sut.GetShows(2, 2, CancellationToken.None)).ToArray();

        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(shows[2]);
        result.Should().ContainEquivalentOf(shows[3]);
        
    }

    public Task InitializeAsync()
        => _mongoDbContainer.StartAsync();

    public Task DisposeAsync()
        => _mongoDbContainer.DisposeAsync().AsTask();
}