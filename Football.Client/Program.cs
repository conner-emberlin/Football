using Football.Client;
using Football.Client.Interfaces;
using Football.Client.Services;
using Football.Client.Helpers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IRequests, Requests>();
builder.Services.AddScoped<IFantasyService, FantasyService>();
builder.Services.AddScoped<IOperationsService, OperationsService>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IProjectionService, ProjectionService>();
builder.Services.AddScoped<ITeamsService, TeamsService>();



var app = builder.Build();
await builder.Build().RunAsync();
