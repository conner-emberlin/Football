using Football.Api.Helpers;
using Football.Interfaces;
using Football.Services;
using Football.Repository;
using System.Runtime.CompilerServices;
using System.Data;
using System.Data.SqlClient;

var allowedOrigins = "_allowedOrigins";
var builder = WebApplication.CreateBuilder(args);

//inject connection string to controllers
string dboFoootballConnectionString = builder.Configuration.GetConnectionString("dboFootballConnectionString");

//allow redirects to UI url
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins, policy => policy.WithOrigins("https://localhost:44480/"));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISqlQueryService, SqlQueryService>();
builder.Services.AddScoped<IFantasyService, FantasyService>();
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddScoped<IMatrixService, MatrixService>();
builder.Services.AddScoped<IPerformRegressionService, PerformRegressionService>();
builder.Services.AddScoped<IRegressionModelService, RegressionModelService>();
builder.Services.AddScoped<IServiceHelper, ServiceHelper>();
builder.Services.AddScoped<IFantasyRepository, FantasyRepository>();
builder.Services.AddScoped<IRegressionModelRepository, RegressionModelRepository>();
builder.Services.AddScoped<IDbConnection>((sp => new SqlConnection(dboFoootballConnectionString)));
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
