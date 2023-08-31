using Football.Api.Helpers;
using Football.Interfaces;
using Football.Services;
using Football.Repository;
using News.Interfaces;
using News.Services;
using Football.Models;
using System.Data;
using System.Data.SqlClient;
using Serilog;
using Microsoft.Extensions.Caching.Memory;

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
builder.Services.AddScoped<IDbConnection>((sp => new SqlConnection(dboFoootballConnectionString)));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Serilog.ILogger>(log);


builder.Services.Configure<Season>(builder.Configuration.GetSection("Season"));
builder.Services.Configure<ReplacementLevels>(builder.Configuration.GetSection("ReplacementLevels"));
builder.Services.Configure<FantasyScoring>(builder.Configuration.GetSection("FantasyScoring"));
builder.Services.Configure<Projections>(builder.Configuration.GetSection("Projections"));
builder.Services.Configure<Starters>(builder.Configuration.GetSection("Starters"));
builder.Services.Configure<Tunings>(builder.Configuration.GetSection("Tunings"));

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
