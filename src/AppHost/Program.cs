using Application.Extensions;
using Hangfire;
using Infrastructure.Extensions;
using Infrastructure.Telegram;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", optional: true);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure();
builder.Services.AddHostedService<TelegramBotHostedService>();

var app = builder.Build();

// Hangfire Dashboard at /hangfire
app.MapHangfireDashboard("/hangfire");

app.Run();
