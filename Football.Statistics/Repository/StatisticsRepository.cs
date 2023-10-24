using Football.Data.Models;
using Football.Statistics.Interfaces;
using System.Data;
using Dapper;
using Football.Enums;

namespace Football.Statistics.Repository
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly IDbConnection _dbConnection;

        public StatisticsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<T>> GetWeeklyData<T>(PositionEnum position, int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].[{GetWeeklyTable(position)}] WHERE [Season] = @season
                                        AND [Week] = @week";
            return (await _dbConnection.QueryAsync<T>(query, new { season, week })).ToList();
        }
        public async Task<List<T>> GetWeeklyData<T>(PositionEnum position, int playerId)
        {
            var query = $@"SELECT * FROM [dbo].[{GetWeeklyTable(position)}] WHERE [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<T>(query, new { playerId })).ToList();
        }
        public async Task<List<T>> GetSeasonData<T>(PositionEnum position, int queryParam, bool isPlayer)
        {
            var query = $@"SELECT * FROM [dbo].[{GetSeasonTable(position)}] WHERE {GetSeasonQueryWhere(isPlayer)}";
            return (await _dbConnection.QueryAsync<T>(query, new { queryParam })).ToList();
        }

        public async Task<List<GameResult>> GetGameResults(int season)
        {
            var query = $@"SELECT * FROM [dbo].GameResults WHERE [Season] = @season";                                               
            return (await _dbConnection.QueryAsync<GameResult>(query, new { season})).ToList();
        }

        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyRosterPercentages
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return (await _dbConnection.QueryAsync<WeeklyRosterPercent>(query, new { season, week })).ToList();
        }

        private static string GetWeeklyTable(PositionEnum position) => string.Format("Weekly{0}Data", position.ToString());
        private static string GetSeasonTable(PositionEnum position) => string.Format("Season{0}Data", position.ToString());
        private static string GetSeasonQueryWhere(bool isPlayer) => isPlayer ? "[PlayerId] = @queryParam" : "[Season] = @queryParam";

    }
}

