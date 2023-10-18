using Football.Data.Models;
using Football.Fantasy.Interfaces;
using System.Data;
using Dapper;

namespace Football.Fantasy.Repository
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly IDbConnection _dbConnection;

        public StatisticsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<SeasonDataQB>> GetSeasonDataQBBySeason(int season)
        {
            var query = $@"SELECT 
		                     [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Completions]
                            ,[Attempts]
                            ,[Yards]
                            ,[TD]
                            ,[Int]
                            ,[Sacks]
                            ,[RushingAttempts]
                            ,[RushingYards]
                            ,[RushingTD]
                            ,[Fumbles]
                            ,[Games]
                        FROM [dbo].SeasonQBData
                        WHERE 1=1
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataQB>(query, new { season })).ToList();
        }
        public async Task<List<SeasonDataQB>> GetSeasonDataQB(int playerId)
        {
            var query = $@"SELECT 
		                     [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Completions]
                            ,[Attempts]
                            ,[Yards]
                            ,[TD]
                            ,[Int]
                            ,[Sacks]
                            ,[RushingAttempts]
                            ,[RushingYards]
                            ,[RushingTD]
                            ,[Fumbles]
                            ,[Games]
                        FROM [dbo].SeasonQBData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataQB>(query, new { playerId })).ToList();
        }
        public async Task<List<SeasonDataRB>> GetSeasonDataRBBySeason(int season)
        {
            var query = $@"SELECT [Season]
                         ,[PlayerID]
                         ,[Name]
                         ,[RushingAtt]
                         ,[RushingYds]
                         ,[RushingTD]
                         ,[Receptions]
                         ,[Targets]
                         ,[Yards]
                         ,[ReceivingTD]
                         ,[Fumbles]
                         ,[Games]
                        FROM [dbo].SeasonRBData
                        WHERE 1=1
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataRB>(query, new { season })).ToList();
        }
        public async Task<List<SeasonDataRB>> GetSeasonDataRB(int playerId)
        {
            var query = $@"SELECT [Season]
                         ,[PlayerID]
                         ,[Name]
                         ,[RushingAtt]
                         ,[RushingYds]
                         ,[RushingTD]
                         ,[Receptions]
                         ,[Targets]
                         ,[Yards]
                         ,[ReceivingTD]
                         ,[Fumbles]
                         ,[Games]
                        FROM [dbo].SeasonRBData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataRB>(query, new { playerId })).ToList();
        }
        public async Task<List<SeasonDataWR>> GetSeasonDataWRBySeason(int season)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                            ,[Games]
                        FROM [dbo].SeasonWRData
                        WHERE 1=1
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataWR>(query, new { season })).ToList();
        }
        public async Task<List<SeasonDataWR>> GetSeasonDataWR(int playerId)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                            ,[Games]
                        FROM [dbo].SeasonWRData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataWR>(query, new { playerId })).ToList();
        }
        public async Task<List<SeasonDataTE>> GetSeasonDataTEBySeason(int season)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                            ,[Games]
                        FROM [dbo].SeasonTEData
                        WHERE 1=1
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataTE>(query, new { season })).ToList();
        }
        public async Task<List<SeasonDataTE>> GetSeasonDataTE(int playerId)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                            ,[Games]
                        FROM [dbo].SeasonTEData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataTE>(query, new { playerId })).ToList();
        }
        public async Task<List<SeasonDataDST>> GetSeasonDataDSTBySeason(int season)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Sacks]
                            ,[Ints]
                            ,[FumblesRecovered]
                            ,[ForcedFumbles]
                            ,[DefensiveTD]
                            ,[Safties]
                            ,[SpecialTD]
                            ,[Games]
                        FROM [dbo].SeasonDSTData
                        WHERE 1=1
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataDST>(query, new { season })).ToList();
        }
        public async Task<List<SeasonDataDST>> GetSeasonDataDST(int playerId)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[PlayerID]
                            ,[Name]
                            ,[Sacks]
                            ,[Ints]
                            ,[FumblesRecovered]
                            ,[ForcedFumbles]
                            ,[DefensiveTD]
                            ,[Safties]
                            ,[SpecialTD]
                            ,[Games]
                        FROM [dbo].SeasonDSTData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataDST>(query, new { playerId })).ToList();
        }

        public async Task<List<WeeklyDataQB>> GetWeeklyDataQB(int season, int week)
        {
            var query = $@"SELECT 
		                     [Season]
                            ,[Week]
                            ,[PlayerID]
                            ,[Name]
                            ,[Completions]
                            ,[Attempts]
                            ,[Yards]
                            ,[TD]
                            ,[Int]
                            ,[Sacks]
                            ,[RushingAttempts]
                            ,[RushingYards]
                            ,[RushingTD]
                            ,[Fumbles]
                        FROM [dbo].WeeklyQBData
                        WHERE 1=1
                        AND [Season] = @season
                        AND [Week] = @week
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataQB>(query, new { season, week })).ToList();
        }
        public async Task<List<WeeklyDataQB>> GetWeeklyDataQB(int playerId)
        {
            var query = $@"SELECT 
		                     [Season]
                            ,[Week]
                            ,[PlayerID]
                            ,[Name]
                            ,[Completions]
                            ,[Attempts]
                            ,[Yards]
                            ,[TD]
                            ,[Int]
                            ,[Sacks]
                            ,[RushingAttempts]
                            ,[RushingYards]
                            ,[RushingTD]
                            ,[Fumbles]
                        FROM [dbo].WeeklyQBData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataQB>(query, new { playerId })).ToList();
        }
        public async Task<List<WeeklyDataRB>> GetWeeklyDataRB(int season, int week)
        {
            var query = $@"SELECT 
                         [Season]
                         ,[Week]
                         ,[PlayerID]
                         ,[Name]
                         ,[RushingAtt]
                         ,[RushingYds]
                         ,[RushingTD]
                         ,[Receptions]
                         ,[Targets]
                         ,[Yards]
                         ,[ReceivingTD]
                         ,[Fumbles]
                        FROM [dbo].WeeklyRBData
                        WHERE 1=1
                        AND [Season] = @season
                        AND [Week] = @week
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataRB>(query, new { season, week })).ToList();
        }
        public async Task<List<WeeklyDataRB>> GetWeeklyDataRB(int playerId)
        {
            var query = $@"SELECT 
                         [Season]
                         ,[Week]
                         ,[PlayerID]
                         ,[Name]
                         ,[RushingAtt]
                         ,[RushingYds]
                         ,[RushingTD]
                         ,[Receptions]
                         ,[Targets]
                         ,[Yards]
                         ,[ReceivingTD]
                         ,[Fumbles]
                        FROM [dbo].WeeklyRBData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataRB>(query, new { playerId })).ToList();
        }
        public async Task<List<WeeklyDataWR>> GetWeeklyDataWR(int season, int week)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[Week]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                        FROM [dbo].WeeklyWRData
                        WHERE 1=1
                        AND [Season] = @season
                        AND [Week] = @week
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataWR>(query, new { season, week })).ToList();
        }
        public async Task<List<WeeklyDataWR>> GetWeeklyDataWR(int playerId)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[Week]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                        FROM [dbo].WeeklyWRData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataWR>(query, new { playerId })).ToList();
        }
        public async Task<List<WeeklyDataTE>> GetWeeklyDataTE(int season, int week)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[Week]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                        FROM [dbo].WeeklyTEData
                        WHERE 1=1
                        AND [Season] = @season
                        AND [Week] = @week
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataTE>(query, new { season, week })).ToList();
        }
        public async Task<List<WeeklyDataTE>> GetWeeklyDataTE(int playerId)
        {
            var query = $@"SELECT 
                            [Season]
                            ,[Week]
                            ,[PlayerID]
                            ,[Name]
                            ,[Receptions]
                            ,[Targets]
                            ,[Yards]
                            ,[Long]
                            ,[TD]
                            ,[RushingAtt]
                            ,[RushingYds]
                            ,[RushingTD]
                            ,[Fumbles]
                        FROM [dbo].WeeklyTEData
                        WHERE 1=1
                        AND [PlayerId] = @playerId
                        ";
            return (await _dbConnection.QueryAsync<WeeklyDataTE>(query, new { playerId })).ToList();
        }
        public async Task<List<WeeklyDataDST>> GetWeeklyDataDST(int season, int week)
        {
            var query = $@"SELECT
                            [Season], [Week], [PlayerId], [Name],
                            [Sacks], [Ints], [FumblesRecovered],
                            [ForcedFumbles], [DefensiveTD], [Safties], [SpecialTD]
                            FROM [dbo].WeeklyDSTData
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return (await _dbConnection.QueryAsync<WeeklyDataDST>(query, new {season, week})).ToList();
        }
        public async Task<List<WeeklyDataDST>> GetWeeklyDataDST(int playerId)
        {
            var query = $@"SELECT
                            [Season], [Week], [PlayerId], [Name],
                            [Sacks], [Ints], [FumblesRecovered],
                            [ForcedFumbles], [DefensiveTD], [Safties], [SpecialTD]
                            FROM [dbo].WeeklyDSTData
                            WHERE 1=1
                                AND [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<WeeklyDataDST>(query, new { playerId})).ToList();
        }
        public async Task<List<GameResult>> GetGameResults(int season, int week)
        {
            var query = $@"SELECT [Season]
                                ,[WinnerId]
                                ,[LoserId]
                                ,[HomeTeamId]
                                ,[AwayTeamId]
                                ,[Week]
                                ,[Day]
                                ,[Date]
                                ,[Time]
                                ,[Winner]
                                ,[Loser]
                                ,[WinnerPoints]
                                ,[LoserPoints]
                                ,[WinnerYards]
                                ,[LoserYards]
                        FROM [dbo].GameResults
                        WHERE [Season] = @season
                        AND [Week] = @week";
            return (await _dbConnection.QueryAsync<GameResult>(query, new { season, week })).ToList();
        }

        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyRosterPercentages
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return (await _dbConnection.QueryAsync<WeeklyRosterPercent>(query, new { season, week })).ToList();
        }
    }
}
