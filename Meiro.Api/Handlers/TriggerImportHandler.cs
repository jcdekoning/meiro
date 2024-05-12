using Meiro.Api.BackgroundJobs;
using Quartz;

namespace Meiro.Api.Handlers;

public static class TriggerImportHandler
{
    public static async Task<IResult> TriggerJob(ISchedulerFactory schedulerFactory,  HttpContext httpContext)
    {
        var scheduler = await schedulerFactory.GetScheduler(httpContext.RequestAborted);
        var jobKey = new JobKey(nameof(ImportShowsJob));
        await scheduler.TriggerJob(jobKey, httpContext.RequestAborted);
        return Results.Ok("Job triggered");
    }
}