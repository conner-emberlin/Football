using Football.Data.Models;
using Football.Statistics.Interfaces;
using System.Data;
using Dapper;
using Football.Enums;

namespace Football.Statistics.Repository
{
    public class StatisticsRepository(IDbConnection dbConnection) : IStatisticsRepository
    {
        public async Task<List<T>> GetWeeklyData<T>(Position position, int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].[{GetWeeklyTable(position)}] WHERE [Season] = @season
                                        AND [Week] = @week";
            return (await dbConnection.QueryAsync<T>(query, new { season, week })).ToList();
        }
        public async Task<List<T>> GetWeeklyData<T>(Position position, int playerId)
        {
            var query = $@"SELECT * FROM [dbo].[{GetWeeklyTable(position)}] WHERE [PlayerId] = @playerId";
            return (await dbConnection.QueryAsync<T>(query, new { playerId })).ToList();
        }
        public async Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer)
        {
            var query = $@"SELECT * FROM [dbo].[{GetSeasonTable(position)}] WHERE {GetSeasonQueryWhere(isPlayer)}";
            return (await dbConnection.QueryAsync<T>(query, new { queryParam })).ToList();
        }

        public async Task<List<GameResult>> GetGameResults(int season)
        {
            var query = $@"SELECT * FROM [dbo].GameResults WHERE [Season] = @season";                                               
            return (await dbConnection.QueryAsync<GameResult>(query, new { season})).ToList();
        }

        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyRosterPercentages
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return (await dbConnection.QueryAsync<WeeklyRosterPercent>(query, new { season, week })).ToList();
        }

        private static string GetWeeklyTable(Position position) => string.Format("Weekly{0}Data", position.ToString());
        private static string GetSeasonTable(Position position) => string.Format("Season{0}Data", position.ToString());
        private static string GetSeasonQueryWhere(bool isPlayer) => isPlayer ? "[PlayerId] = @queryParam" : "[Season] = @queryParam";

    }
}

