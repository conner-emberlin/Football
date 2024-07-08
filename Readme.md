- Multi-Regression models to predict both season total fantasy points and weekly predictions
- In Season Fantasy tools - Start or Sit Assistant, Positional Matchup Analysis, Waiver Wire Assistant etc.
- Scraper to retrieve data from the web and insert into an SQL Server database. Scraper is accessible through the API
- Blazor UI
  
Adding a variable to regression models:
1. Add to the correpsonding classes in RegressionModels.cs
2. If the variable only uses Season/Weekly data, populate it in the AutomapperProfile. Otherwise, populate in the corresponding Projection service.
3. If the variable requires averaging, do so in StatProjectionCalculator.cs

APIs:
Weekly 
1. Upload weekly statistics: POST /api/uploaddata/{position}/{season}/{week}
2. Upload weekly game results: POST /api/uploaddata/game-results/{season}/{week}
3. Upload weekly fantasy: POST /api/fantasy/data/{position}/{season}/{week}
4. Upload weekly roster percentages: POST /api/uploaddata/roster-percent/{position}/{season}/{week}
5. Add any injuries from the prior week: POST/api/players/injury 
6. Upload weekly projections: POST /api/projection/weekly/{position}

Preseason
1. Upload Season data from previous season: POST /api/uploaddata/{position}/{season}
2. Upload season fantasy: POST /api/fantasy/{position}/{season}
3. Flip CurrentSeason appsetting to new season.
4. Upload schedule: POST /api/uploaddata/schedule
5. Upload new player teams: POST /api/uploaddata/teams/{position}
6. Upload season projections: POST /api/projection/season/{position} 

As Needed
1. Update player headshots: POST /api/uploaddata/headshots/{position}
2. Update team logos: POST /api/uploaddata/logos

External APIs
1. WeatherAPI: https://www.weatherapi.com/docs/
2. BettingOdds: https://the-odds-api.com/liveapi/guides/v4/#overview
3. ESPN: https://gist.github.com/akeaswaran/b48b02f1c94f873c6655e7129910fc3b
4. Sleeper: https://docs.sleeper.com/

