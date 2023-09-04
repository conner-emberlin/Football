using Football.Api.Helpers;
using Football.Interfaces;
using Football.Data.Services;
using Football.Data.Interfaces;
using Football.Services;
using Football.Repository;
using News.Interfaces;
using News.Services;
using Football.Models;
using System.Data;
using System.Data.SqlClient;
using Serilog;
using Microsoft.Extensions.Caching.Memory;
using Football.Data.Repository;
using Football.Data.Models;
using News.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Repository;
using Football.Fantasy.Services;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7176/");
                      });
});

string dboFoootballConnectionString = builder.Configuration.GetConnectionString("dboFootballConnectionString");
using var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341/").CreateLogger();
MemoryCacheEntryOptions cache = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(120)).SetAbsoluteExpiration(TimeSpan.FromSeconds(3600)).SetPriority(CacheItemPriority.Normal);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISqlQueryService, SqlQueryService>();
builder.Services.AddScoped<IFantasyService, FantasyService>();
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddScoped<IMatrixService, MatrixService>();
builder.Services.AddScoped<IPerformRegressionService, PerformRegressionService>();
builder.Services.AddScoped<IRegressionModelService, RegressionModelService>();
builder.Services.AddScoped<IServiceHelper, ServiceHelper>();
builder.Services.AddScoped<IFantasyRepository, FantasyRepository>();
builder.Services.AddScoped<IDataUploadService, DataUploadService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IWeightedAverageCalculator, WeightedAverageCalculator>();
builder.Services.AddScoped<IDataUploadRepository, DataUploadRepository>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IAdjustmentCalculator, AdjustmentCalculator>();
builder.Services.AddScoped<IAdjustmentRepository, AdjustmentRepository>();
builder.Services.AddScoped<IScraperService, ScraperService>();
builder.Services.AddScoped<IUploadWeeklyDataService, UploadWeeklyDataService>();
builder.Services.AddScoped<IUploadWeeklyDataRepository, UploadWeeklyDataRepository>();
builder.Services.AddScoped<IUploadSeasonDataService, UploadSeasonDataService>();
builder.Services.AddScoped<IUploadSeasonDataRepository, UploadSeasonDataRepository>();
builder.Services.AddScoped<IFantasyCalculator, FantasyCalculator>();
builder.Services.AddScoped<IFantasyDataRepository, FantasyDataRepository>();
builder.Services.AddScoped<IFantasyDataService, FantasyDataService>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IDbConnection>((sp => new SqlConnection(dboFoootballConnectionString)));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Serilog.ILogger>(log);


builder.Services.Configure<Season>(builder.Configuration.GetSection("Season"));
builder.Services.Configure<ReplacementLevels>(builder.Configuration.GetSection("ReplacementLevels"));
builder.Services.Configure<FantasyScoring>(builder.Configuration.GetSection("FantasyScoring"));
builder.Services.Configure<Projections>(builder.Configuration.GetSection("Projections"));
builder.Services.Configure<Starters>(builder.Configuration.GetSection("Starters"));
builder.Services.Configure<Tunings>(builder.Configuration.GetSection("Tunings"));
builder.Services.Configure<WeeklyScraping>(builder.Configuration.GetSection("Scraping"));
builder.Services.Configure<ESPN>(builder.Configuration.GetSection("ESPN"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
