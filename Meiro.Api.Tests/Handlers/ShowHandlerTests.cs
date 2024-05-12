using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Meiro.Api.Handlers;
using Meiro.Api.Mappers;
using Meiro.Application.Orchestrators;
using Meiro.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Meiro.Api.Tests.Handlers;

public class ShowHandlerTests
{
    private readonly Mock<IGetShowsOrchestrator> _orchestratorMock = new();
    private readonly Mock<IShowMapper> _mapperMock = new();

    [Theory, AutoData]
    public async Task GetShows_ShouldCallTheOrchestrator_WithPageAndPageSize(int page, int pageSize)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var context = new DefaultHttpContext { RequestAborted = token };
        
        await ShowHandler.GetShows(_orchestratorMock.Object, _mapperMock.Object, context, page, pageSize);
        
        _orchestratorMock.Verify(o => o.GetShows(page, pageSize, token));
        _orchestratorMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetShows_ShouldCallTheOrchestrator_WithDefaultPageAndPageSize_WhenNotDefined()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var context = new DefaultHttpContext { RequestAborted = token };
        
        await ShowHandler.GetShows(_orchestratorMock.Object, _mapperMock.Object, context);
        
        _orchestratorMock.Verify(o => o.GetShows(1, 10, token));
        _orchestratorMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetShows_ShouldCallTheMapper_ForEachReturnedShow()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        var showOne = fixture.Create<Show>();
        var showTwo = fixture.Create<Show>();
        _orchestratorMock.Setup(o => o.GetShows(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([showOne, showTwo]);
        
        await ShowHandler.GetShows(_orchestratorMock.Object, _mapperMock.Object, new DefaultHttpContext());
        
        _mapperMock.Verify(m => m.MapToContract(showOne), Times.Once);
        _mapperMock.Verify(m => m.MapToContract(showTwo), Times.Once);
        _mapperMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetShows_ReturnsAnOkResultWithTheMappedShows()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        _orchestratorMock.Setup(o => o.GetShows(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([fixture.Create<Show>(), fixture.Create<Show>()]);

        var contractShowOne = fixture.Create<Contract.Show>();
        var contractShowTwo = fixture.Create<Contract.Show>();
        _mapperMock.SetupSequence(m => m.MapToContract(It.IsAny<Show>()))
            .Returns(contractShowOne)
            .Returns(contractShowTwo);
        
        var result = await ShowHandler.GetShows(_orchestratorMock.Object, _mapperMock.Object, new DefaultHttpContext());
        result.Should().BeOfType<Ok<List<Contract.Show>>>();

        var okResult = (Ok<List<Contract.Show>>)result;
        okResult.Value.Should().BeEquivalentTo([contractShowOne, contractShowTwo]);
    }
    
    [Fact]
    public async Task GetShows_ReturnsBadRequest_WhenArgumentOutOfRangeExceptionIsThrown()
    {
        _orchestratorMock.Setup(o => o.GetShows(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentOutOfRangeException());
        
        var result = await ShowHandler.GetShows(_orchestratorMock.Object, _mapperMock.Object, new DefaultHttpContext());
        result.Should().BeOfType<BadRequest<string>>();
    }
}