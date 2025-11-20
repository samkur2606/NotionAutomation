using Hangfire;
using Hangfire.Storage.SQLite;
using NotionAutomation.Api.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var hangfireConnection = builder.Configuration.GetConnectionString("Hangfire");
DirectoryHelper.CreateFolderIfNecessary(hangfireConnection);
builder.Services.AddHangfire(config => { config.UseSQLiteStorage(hangfireConnection); });
builder.Services.AddControllers();
builder.Services.AddHangfireServer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();