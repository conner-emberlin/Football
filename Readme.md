Weekly Operations
1. Upload weekly statistics: POST /api/uploaddata/{position}/{season}/{week} - Run for positions QB, RB, WR, TE, DST
2. Upload weekly game results: POST /api/uploaddata/game-results/{season}/{week}
3. Upload weekly fantasy: POST /api/fantasy/data/{position}/{season}/{week}
4. Run and upload weekly projections: GET/POST /api/projection/weekly/{position}

As Needed
1. Update player headshots: POST /api/uploaddata/headshots/{position}
2. Update team logos: POST /api/uploaddata/logos

Preseason
1. Upload Season data from previous season: POST /api/uploaddata/{position}/{season}
2. Upload season fantasy: POST /api/fantasy/{position}/{season}
3. Flip CurrentSeason appsetting to new season.
4. Upload schedule: POST /api/uploaddata/schedule
5. Upload new player teams: POST /api/uploaddata/teams/{position}
6. Run and upload season projections: GET/POST /api/projection/season/{position} 