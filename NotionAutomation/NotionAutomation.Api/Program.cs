using Hangfire;
using Hangfire.Storage.SQLite;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Jobs;
using NotionAutomation.Api.Logic;
using NotionAutomation.Api.Models;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.Get<AppSettings>() ?? throw new Exception("AppSettings missing");

// Add services to the container.
var hangfireConnection = builder.Configuration.GetConnectionString("Hangfire");
DirectoryHelper.CreateFolderIfNecessary(hangfireConnection);
builder.Services.AddHangfire(config => { config.UseSQLiteStorage(hangfireConnection); });
builder.Services.AddControllers();
builder.Services.AddHangfireServer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton<NotionService>();
builder.Services.AddTransient<TestJob>();

var app = builder.Build();

// TEMP
var notionService = app.Services.GetRequiredService<NotionService>();
await notionService.TestCall();
//

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireUiInEnglish();
app.UseHangfireDashboard();
app.RegisterRecurringJobs();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();