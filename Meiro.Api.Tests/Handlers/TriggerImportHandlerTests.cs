using FluentAssertions;
using Meiro.Api.BackgroundJobs;
using Meiro.Api.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Quartz;

namespace Meiro.Api.Tests.Handlers;

public class TriggerImportHandlerTests
{
    [Fact]
    public async Task TriggerJob_ShouldTriggerImportShowsJob()
    {
        var schedulerMock = new Mock<IScheduler>();
        var schedulerFactoryMock = new Mock<ISchedulerFactory>();
        schedulerFactoryMock.Setup(s => s.GetScheduler(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedulerMock.Object);

        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var context = new DefaultHttpContext { RequestAborted = token };
        
        await TriggerImportHandler.TriggerJob(schedulerFactoryMock.Object, context);
        
        var jobKey = new JobKey(nameof(ImportShowsJob));
        schedulerMock.Verify(s => s.TriggerJob(jobKey, token), Times.Once);
        schedulerMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task TriggerJob_ReturnsOkResult()
    {
        var schedulerMock = new Mock<IScheduler>();
        var schedulerFactoryMock = new Mock<ISchedulerFactory>();
        schedulerFactoryMock.Setup(s => s.GetScheduler(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedulerMock.Object);
        
        var result = await TriggerImportHandler.TriggerJob(schedulerFactoryMock.Object, new DefaultHttpContext());
        
        result.Should().BeOfType<Ok<string>>();
        var okResult = (Ok<string>)result;
        okResult.Value.Should().Be("Job triggered");
    }
}