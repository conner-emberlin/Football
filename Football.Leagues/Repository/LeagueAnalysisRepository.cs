using Dapper;
using Football.Leagues.Interfaces;
using Football.Leagues.Models;
using System.Data;

namespace Football.Leagues.Repository
{
    public class LeagueAnalysisRepository(IDbConnection connection) : ILeagueAnalysisRepository
    {
        public async Task<int> UploadSleeperPlayerMap(List<SleeperPlayerMap> playerMap)
        {
            var query = $@"INSERT INTO [dbo].SleeperPlayerMap (SleeperPlayerId, PlayerId) VALUES (@SleeperPlayerId, @PlayerId)";
            return await connection.ExecuteAsync(query, playerMap);
        }
        public async Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId)
        {
            var query = $@"SELECT * FROM [dbo].SleeperPlayerMap WHERE SleeperPlayerId = @sleeperId";
            return (await connection.QueryAsync<SleeperPlayerMap>(query, new { sleeperId })).FirstOrDefault();
        }
    }
}
