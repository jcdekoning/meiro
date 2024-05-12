using System.Net;
using AutoFixture;
using FluentAssertions;
using Meiro.Application.ExternalServices;
using Meiro.Infrastructure.Mappers;
using Meiro.Infrastructure.TvMaze;
using Meiro.Infrastructure.TvMaze.Contract;
using Moq;
using Refit;

namespace Meiro.Infrastructure.Tests;

public class TvInformationClientTests
{
    private readonly Mock<ITvMazeClient> _mazeTvClientMock;
    private readonly Mock<IShowMapper> _showMapperMock;
    private readonly Mock<ICastMapper> _castMapperMock;
    private readonly ITvInformationClient _sut;
    private readonly Fixture _fixture;
    
    public TvInformationClientTests()
    {
        _fixture = new Fixture();
        _fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

        _mazeTvClientMock = new Mock<ITvMazeClient>();
        _showMapperMock = new Mock<IShowMapper>();
        _castMapperMock = new Mock<ICastMapper>();
        _sut = new TvInformationClient(_mazeTvClientMock.Object, _showMapperMock.Object, _castMapperMock.Object);
    }
    
    [Fact]
    public async Task GetShows_CallsTheMazeTvApi()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        
        var pageId = _fixture.Create<int>();

        await _sut.GetShows(pageId, token);
        
        _mazeTvClientMock.Verify(m => m.GetShows(pageId, token), Times.Once);
        _mazeTvClientMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetShows_CallsTheMapperForEachShow()
    {
        var showOne = _fixture.Create<Show>();
        var showTwo = _fixture.Create<Show>();

        _mazeTvClientMock.Setup(m => m.GetShows(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([showTwo, showOne]);

        await _sut.GetShows(_fixture.Create<int>(), CancellationToken.None);
        
        _showMapperMock.Verify(m => m.MapToApplication(showOne), Times.Once);
        _showMapperMock.Verify(m => m.MapToApplication(showTwo), Times.Once);
        _showMapperMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetShows_ReturnsTheMappedResult()
    {
        _mazeTvClientMock.Setup(m => m.GetShows(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_fixture.Create<Show>(), _fixture.Create<Show>()]);

        var showOne = _fixture.Create<Application.Models.Show>();
        var showTwo = _fixture.Create<Application.Models.Show>();
        _showMapperMock.SetupSequence(m => m.MapToApplication(It.IsAny<Show>()))
            .Returns(showOne)
            .Returns(showTwo);

        var result = await _sut.GetShows(_fixture.Create<int>(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain([showTwo, showOne]);
    }
    
    [Fact]
    public async Task GetShows_ReturnsEmptyResult_WhenHttpStatusCodeNotFound()
    {
        _mazeTvClientMock.Setup(m => m.GetShows(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(await ApiException.Create(new HttpRequestMessage(), HttpMethod.Get,
                new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound }, new RefitSettings()));

        var result = await _sut.GetShows(_fixture.Create<int>(), CancellationToken.None);

        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetCastForShow_CallsTheMazeTvApi()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        
        var showId = _fixture.Create<int>();

        await _sut.GetCastForShow(showId, token);
        
        _mazeTvClientMock.Verify(m => m.GetShowCast(showId, token), Times.Once);
        _mazeTvClientMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetCastForShow_CallsTheMapperForEachCast()
    {
        var castOne = _fixture.Create<Cast>();
        var castTwo = _fixture.Create<Cast>();

        _mazeTvClientMock.Setup(m => m.GetShowCast(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([castTwo, castOne]);

        await _sut.GetCastForShow(_fixture.Create<int>(), CancellationToken.None);
        
        _castMapperMock.Verify(m => m.MapToApplication(castOne), Times.Once);
        _castMapperMock.Verify(m => m.MapToApplication(castTwo), Times.Once);
        _castMapperMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetCastForShow_ReturnsTheMappedResult()
    {
        _mazeTvClientMock.Setup(m => m.GetShowCast(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([_fixture.Create<Cast>(), _fixture.Create<Cast>()]);

        var castOne = _fixture.Create<Application.Models.Cast>();
        var castTwo = _fixture.Create<Application.Models.Cast>();
        _castMapperMock.SetupSequence(m => m.MapToApplication(It.IsAny<Cast>()))
            .Returns(castOne)
            .Returns(castTwo);

        var result = await _sut.GetCastForShow(_fixture.Create<int>(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain([castTwo, castOne]);
    }
}