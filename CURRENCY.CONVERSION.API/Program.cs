using CURRENCY.CONVERSION.API.Extensions;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Jobs;
using CURRENCY.CONVERSION.API.Repositories;
using CURRENCY.CONVERSION.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Repositories (MongoDB)
builder.Services.AddSingleton<ICurrencyRateRepository, CurrencyRateRepository>();
builder.Services.AddSingleton<IConversionHistoryRepository, ConversionHistoryRepository>();

builder.Services.AddHttpClient<ISyncExchangeRatesService, SyncExchangeRatesService>();

// Business Service Layer
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IConversionHistoryService, ConversionHistoryService>();

// Swagger / OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Currency Converter API",
        Description = "A .NET 9 Web API with MongoDB for currency conversion and historical tracking."
    });
});

builder.Services.AddSingleton<IMongoClient>(
    s => new MongoClient(builder.Configuration["MongoDbSettings:ConnectionString"]));

// Register Quartz services
builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<SyncExchangeRatesJob>("SyncExchangeRatesJob", 60);
});

// Register Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Keep default console logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure Serilog only for MongoDB
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Quartz", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

// Add Serilog to DI but DON'T override the default logger
builder.Services.AddSingleton(Log.Logger);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Enable Swagger UI in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Converter API v1");
    c.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();