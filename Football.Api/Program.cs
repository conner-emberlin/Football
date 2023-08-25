using Football.Api.Helpers;
using Football.Interfaces;
using Football.Services;
using Football.Repository;
using News.Interfaces;
using News.Services;
using System.Runtime.CompilerServices;
using System.Data;
using System.Data.SqlClient;
using Serilog;
using Serilog.Sinks.Seq;
using Microsoft.Extensions.Caching.Memory;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

//allow redirects to the UI
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7176/");
                      });
});

//inject connection string to controllers
string dboFoootballConnectionString = builder.Configuration.GetConnectionString("dboFootballConnectionString");

//inject Seq loggin
using var log = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341/").CreateLogger();

//inject memory cache
MemoryCacheEntryOptions cache = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(120)).SetAbsoluteExpiration(TimeSpan.FromSeconds(3600)).SetPriority(CacheItemPriority.Normal);

// Add services to the container.
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
