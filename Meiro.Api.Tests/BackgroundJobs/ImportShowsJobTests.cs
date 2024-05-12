using Meiro.Api.BackgroundJobs;
using Meiro.Application.Orchestrators;
using Moq;
using Quartz;

namespace Meiro.Api.Tests.BackgroundJobs;

public class ImportShowsJobTests
{
    [Fact]
    public async Task Execute_ShouldCallTheOrchestrator()
    {
        var orchestratorMock = new Mock<IImportShowsOrchestrator>();
        var contextMock = new Mock<IJobExecutionContext>();
        var sut = new ImportShowsJob(orchestratorMock.Object);

        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        contextMock.SetupGet(c => c.CancellationToken).Returns(token);

        await sut.Execute(contextMock.Object);
        
        orchestratorMock.Verify(o => o.Import(token), Times.Once);
        orchestratorMock.VerifyNoOtherCalls();
    }
}