using Football;
using Football.Api;
using Football.Models;
using Football.Data.Interfaces;
using Football.Data.Repository;
using Football.Data.Services;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Repository;
using Football.Fantasy.Services;
using Football.Players.Interfaces;
using Football.Players.Repository;
using Football.Players.Services;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Projections.Repository;
using Football.Projections.Services;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using Football.Fantasy.Models;


var builder = WebApplication.CreateBuilder(args);

string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                      });
});

string dboFoootballConnectionString = builder.Configuration.GetConnectionString("dboFootballConnectionString");
using var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341/").CreateLogger();
MemoryCacheEntryOptions cache = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(120)).SetAbsoluteExpiration(TimeSpan.FromSeconds(3600)).SetPriority(CacheItemPriority.Normal);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

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
builder.Services.AddScoped<IStatProjectionCalculator, StatProjectionCalculator>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IPlayersRepository, PlayersRepository>();
builder.Services.AddScoped<IAdjustmentService, AdjustmentService>();
builder.Services.AddScoped<IProjectionRepository, ProjectionRepository>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<IMatchupAnalysisService, MatchupAnalysisService>();
builder.Services.AddScoped<IMatchupAnalysisRepository, MatchupAnalysisRepository>();
builder.Services.AddScoped<IMarketShareService, MarketShareService>();
builder.Services.AddScoped<IStartOrSitService, StartOrSitService>();
builder.Services.AddScoped<IProjectionAnalysisService, ProjectionAnalysisService>();
builder.Services.AddScoped<IWaiverWireService, WaiverWireService>();
builder.Services.AddScoped<IFantasyAnalysisService, FantasyAnalysisService>();
builder.Services.AddScoped<IProjectionService<SeasonProjection>, SeasonProjectionService>();
builder.Services.AddScoped<IProjectionService<WeekProjection>, WeeklyProjectionService>();
builder.Services.AddScoped<ISleeperLeagueService, SleeperLeagueService>();
builder.Services.AddScoped<IDistanceService, DistanceService>();
builder.Services.AddScoped<IAdvancedStatisticsService, AdvancedStatisticsService>();
builder.Services.AddScoped<ISnapCountService, SnapCountService>();
builder.Services.AddScoped<IWeeklyAdjustmentService, WeeklyAdjustmentService>();
builder.Services.AddScoped<ITeamsService, TeamsService>();
builder.Services.AddScoped<ITeamsRepository, TeamsRepository>();
builder.Services.AddScoped<IProjectionModelCalculator, ProjectionModelCalculator>();
builder.Services.AddScoped(typeof(IArtificialNeuralNetworkService<>), typeof(ArtificialNeuralNetwork<>));
builder.Services.AddScoped<IANNWeeklyProjectionService, ANNWeeklyProjectionService>();
builder.Services.AddScoped<IDbConnection>((sp => new SqlConnection(dboFoootballConnectionString)));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Serilog.ILogger>(log);
builder.Services.AddSingleton<JsonOptions>();

builder.Services.Configure<Season>(builder.Configuration.GetSection("Season"));
builder.Services.Configure<FantasyScoring>(builder.Configuration.GetSection("FantasyScoring"));
builder.Services.Configure<WeeklyScraping>(builder.Configuration.GetSection("Scraping"));
builder.Services.Configure<WaiverWireSettings>(builder.Configuration.GetSection("WaiverWireSettings"));
builder.Services.Configure<BoomBustSettings>(builder.Configuration.GetSection("BoomBustSettings"));
builder.Services.Configure<SleeperSettings>(builder.Configuration.GetSection("SleeperSettings"));
builder.Services.Configure<GeoDistance>(builder.Configuration.GetSection("GeoDistance"));
builder.Services.Configure<FiveThirtyEightValueSettings>(builder.Configuration.GetSection("FiveThirtyEightValueSettings"));
builder.Services.Configure<StartOrSitSettings>(builder.Configuration.GetSection("StartOrSitSettings"));
builder.Services.Configure<NFLOddsAPI>(builder.Configuration.GetSection("NFLOddsAPI"));
builder.Services.Configure<WeatherAPI>(builder.Configuration.GetSection("WeatherAPI"));
builder.Services.Configure<ANNConfiguration>(builder.Configuration.GetSection("ANNConfiguration"));
builder.Services.Configure<FantasyAnalysisSettings>(builder.Configuration.GetSection("FantasyAnalysisSettings"));

builder.Services.AddAutoMapper(
    typeof(ApiModelAutomapperProfile), 
    typeof(Football.Data.AutomapperProfile),
    typeof(Football.Fantasy.AutomapperProfile),
    typeof(Football.Players.AutomapperProfile),
    typeof(Football.Projections.AutomapperProfile)
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
