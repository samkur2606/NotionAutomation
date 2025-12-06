using Hangfire;
using Hangfire.Storage.SQLite;
using NotionAutomation.Api.Converters;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Logic;
using NotionAutomation.Api.Models;
using NotionAutomation.Api.Scheduling;
using NotionAutomation.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.Get<AppSettings>() ?? throw new Exception("AppSettings missing");

// todo(sk): hangfire configuration clean up
var hangfireConnection = builder.Configuration.GetConnectionString("Hangfire");
DirectoryHelper.CreateFolderIfNecessary(hangfireConnection);
builder.Services.AddHangfire(config => { config.UseSQLiteStorage(hangfireConnection); });
builder.Services.AddControllers();
builder.Services.AddHangfireServer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton<ConfigurationHelper>();
builder.Services.AddTransient<TestJob>();
builder.Services.AddTransient<NotionParser>();
builder.Services.AddTransient<NotionMapper>();
builder.Services.AddTransient<NotionPageUpdateBuilder>();
builder.Services.AddTransient<NotionDatabaseService>();
builder.Services.AddTransient<NotionPageService>();
builder.Services.AddTransient<NotionRawApiService>();
builder.Services.AddTransient<TimeSheetManager>();
builder.Services.AddTransient<NotionRawParser>();
builder.Services.AddHttpClient();
builder.Services.AddCustomNotionClient();
builder.AddCustomLoggingConfiguration(appSettings);

var app = builder.Build();

// TEMP
var test = app.Services.GetRequiredService<ILogger<Program>>();
test.LogInformation("Was geht ab {test}", "das ist ein Test");
test.LogError("Das ist ein Fehler was geht ab");
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