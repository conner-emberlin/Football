using Football.UI.Helpers;
using Football.UI.Interfaces;
using Football.UI.Services;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy => { policy.WithOrigins("https://localhost:7028"); });
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRequests, Requests>();
builder.Services.AddScoped<IFantasyService, FantasyService>();
builder.Services.AddScoped<IOperationsService, OperationsService>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IProjectionService, ProjectionService>();
builder.Services.AddScoped<ITeamsService, TeamsService>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
