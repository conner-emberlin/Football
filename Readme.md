- Multi-Regression models to predict both season total fantasy points and in-season weekly totals
- In Season Fantasy tools - Start or Sit Assistant, Positional Matchup Analysis, Waiver Wire Assistant etc.
- Scraper to retrieve data from the web and insert into an SQL Server database. Scraper is accessible through the API
- Blazor UI
  
External APIs
1. WeatherAPI: https://www.weatherapi.com/docs/
2. BettingOdds: https://the-odds-api.com/liveapi/guides/v4/#overview
3. ESPN: https://gist.github.com/akeaswaran/b48b02f1c94f873c6655e7129910fc3b
4. Sleeper: https://docs.sleeper.com/

Adding new Season Adjustment
1. Add enum value to Football.Enums.Adjustment.cs
2. Add record to AdjustmentDescription SQL table
3. Alter SeasonAdjustments SQL table - Add new INT column NameAdjustment default to 0
4. Add new column name to Football.Models.SeasonAdjustment class
5. Add column to UploadSeasonAdjustments method in SettingsRepository 
6. Add method to AdjustmentService.cs to calculate adjustment
7. Add if statement to PositionSeasonProjectionAdjustments method in AdjustmentService
8. Alter SeasonAdjustments table in ManageSettings.razor
