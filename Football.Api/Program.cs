using Football.Models;
using Football.Data.Interfaces;
using Football.Data.Models;
using Football.Data.Repository;
using Football.Data.Services;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Repository;
using Football.Fantasy.Services;
using Football.News.Interfaces;
using Football.News.Models;
using Football.News.Services;
using Football.Players.Interfaces;
using Football.Players.Repository;
using Football.Players.Services;
using Football.Projections.Interfaces;
using Football.Projections.Repository;
using Football.Projections.Services;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using Football;

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
builder.Services.AddScoped<INewsService, NewsService>();
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
builder.Services.AddScoped<IMatrixCalculator, MatrixCalculator>();
builder.Services.AddScoped<IProjectionService, ProjectionService>();
builder.Services.AddScoped<IRegressionService, RegressionService>();
builder.Services.AddScoped<IStatProjectionCalculator, StatProjectionCalculator>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
builder.Services.AddScoped<IAdjustmentService, AdjustmentService>();
builder.Services.AddScoped<IProjectionRepository, ProjectionRepository>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IDbConnection>((sp => new SqlConnection(dboFoootballConnectionString)));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Serilog.ILogger>(log);


builder.Services.Configure<Season>(builder.Configuration.GetSection("Season"));
builder.Services.Configure<ReplacementLevels>(builder.Configuration.GetSection("ReplacementLevels"));
builder.Services.Configure<FantasyScoring>(builder.Configuration.GetSection("FantasyScoring"));
builder.Services.Configure<ProjectionLimits>(builder.Configuration.GetSection("Projections"));
builder.Services.Configure<Starters>(builder.Configuration.GetSection("Starters"));
builder.Services.Configure<Tunings>(builder.Configuration.GetSection("Tunings"));
builder.Services.Configure<WeeklyTunings>(builder.Configuration.GetSection("WeeklyTunings"));
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
