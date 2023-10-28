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

Adding to a Regression Model
1. Add variable to corresponding model class: Football.Projections.Models.RegressionModels.cs
2. Populate variable in Football.Projections.Services.RegressionModelService.cs

