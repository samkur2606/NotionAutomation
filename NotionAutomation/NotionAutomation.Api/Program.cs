using Hangfire;
using Hangfire.Storage.SQLite;
using NotionAutomation.Api.Helpers;
using NotionAutomation.Api.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var hangfireConnection = builder.Configuration.GetConnectionString("Hangfire");
DirectoryHelper.CreateFolderIfNecessary(hangfireConnection);
builder.Services.AddHangfire(config => { config.UseSQLiteStorage(hangfireConnection); });
builder.Services.AddControllers();
builder.Services.AddHangfireServer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<TestJob>();

var app = builder.Build();

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