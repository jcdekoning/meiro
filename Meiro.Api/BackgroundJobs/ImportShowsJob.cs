using Meiro.Application.Orchestrators;
using Quartz;

namespace Meiro.Api.BackgroundJobs;

[DisallowConcurrentExecution]
public class ImportShowsJob(IImportShowsOrchestrator importShowsOrchestrator) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await importShowsOrchestrator.Import(context.CancellationToken);
    }
}