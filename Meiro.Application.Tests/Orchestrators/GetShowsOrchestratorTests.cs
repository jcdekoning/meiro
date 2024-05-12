using AutoFixture;
using FluentAssertions;
using Meiro.Application.ExternalServices;
using Meiro.Application.Orchestrators;
using Meiro.Domain;
using Moq;

namespace Meiro.Application.Tests.Orchestrators;

public class GetShowsOrchestratorTests
{
    private readonly Mock<IShowRepository> _showRepositoryMock;
    private readonly IGetShowsOrchestrator _sut;
    
    public GetShowsOrchestratorTests()
    {
        _showRepositoryMock = new Mock<IShowRepository>();
        _sut = new GetShowsOrchestrator(_showRepositoryMock.Object);
    }
    
    [Fact]
    public async Task GetShows_ShouldThrowArgumentOutOfRangeException_WhenPageIdIsZero()
    {
        var act = async () => await _sut.GetShows(0, 10, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("Page id should be greater or equal to one (Parameter 'pageId')");
    }
    
    [Fact]
    public async Task GetShows_ShouldThrowArgumentOutOfRangeException_WhenPageSizeIsZero()
    {
        var act = async () => await _sut.GetShows(10, 0, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("Page size should be greater or equal to one (Parameter 'pageSize')");
    }
    
    [Fact]
    public async Task GetShows_ShouldCallTheRepository()
    {
        var fixture = new Fixture();
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var pageId = fixture.Create<int>() + 1;
        var pageSize = fixture.Create<int>() + 1;

        await _sut.GetShows(pageId, pageSize, token);
        
        _showRepositoryMock.Verify(r => r.GetShows(pageId, pageSize, token), Times.Once);
        _showRepositoryMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetShows_ReturnsTheResultFromTheRepository()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

        var showOne = fixture.Create<Show>();
        var showTwo = fixture.Create<Show>();
        _showRepositoryMock.Setup(r => r.GetShows(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Show> { showOne, showTwo });

        var result = (await _sut.GetShows(fixture.Create<int>() + 1, fixture.Create<int>() + 1, CancellationToken.None))
            .ToArray();

        result.Should().HaveCount(2);
        result.Should().Contain([showOne, showTwo]);
    }
}