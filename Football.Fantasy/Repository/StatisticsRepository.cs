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

        public async Task<SeasonDataQB> GetSeasonDataQB(int playerId, int season)
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
                        WHERE [PlayerId] = @playerId
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataQB>(query, new { playerId, season })).First();
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
        public async Task<SeasonDataRB> GetSeasonDataRB(int playerId, int season)
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
                        WHERE [PlayerId] = @playerId
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataRB>(query, new { playerId, season })).First();
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
            return (await _dbConnection.QueryAsync<SeasonDataRB>(query, new {playerId })).ToList();
        }
        public async Task<SeasonDataWR> GetSeasonDataWR(int playerId, int season)
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
                        WHERE [PlayerId] = @playerId
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataWR>(query, new { playerId, season })).First();
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
            return (await _dbConnection.QueryAsync<SeasonDataWR>(query, new {playerId })).ToList();
        }
        public async Task<SeasonDataTE> GetSeasonDataTE(int playerId, int season)
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
                        WHERE [PlayerId] = @playerId
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataTE>(query, new { playerId, season })).First();
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
        public async Task<SeasonDataDST> GetSeasonDataDST(int playerId, int season)
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
                        WHERE [PlayerId] = @playerId
                        AND [Season] = @season
                        ";
            return (await _dbConnection.QueryAsync<SeasonDataDST>(query, new { playerId, season })).First();
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
    }
}
