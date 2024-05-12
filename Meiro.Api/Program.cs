using Meiro.Api;
using Meiro.Api.BackgroundJobs;
using Meiro.Api.Configuration;
using Meiro.Api.Handlers;
using Meiro.Application;
using Meiro.Infrastructure;
using Meiro.Persistence;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddQuartz(q =>
{
    q.AddJob<ImportShowsJob>(cfg => cfg
        .WithIdentity(nameof(ImportShowsJob))
        .StoreDurably());
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var mongoDbSettings = builder.Configuration.GetRequiredSection("MongoDb").Get<MongoDbSettings>()!;
var endpointSettings = builder.Configuration.GetRequiredSection("Endpoints").Get<EndpointSettings>()!;

builder.Services.AddInfrastructure(endpointSettings.TvMaze);
builder.Services.AddPersistence(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName);
builder.Services.AddApplication();
builder.Services.AddApi();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/show", ShowHandler.GetShows);
app.MapPost("/show/trigger", TriggerImportHandler.TriggerJob);

app.Run();