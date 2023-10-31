using Dapper;
using Football.Leagues.Interfaces;
using Football.Leagues.Models;
using System.Data;

namespace Football.Leagues.Repository
{
    public class LeagueAnalysisRepository : ILeagueAnalysisRepository
    {
        private readonly IDbConnection _connection;

        public LeagueAnalysisRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> UploadSleeperPlayerMap(List<SleeperPlayerMap> playerMap)
        {
            var query = $@"INSERT INTO [dbo].SleeperPlayerMap (SleeperPlayerId, PlayerId) VALUES (@SleeperPlayerId, @PlayerId)";
            return await _connection.ExecuteAsync(query, playerMap);
        }
        public async Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId)
        {
            var query = $@"SELECT * FROM [dbo].SleeperPlayerMap WHERE SleeperPlayerId = @sleeperId";
            return (await _connection.QueryAsync<SleeperPlayerMap>(query, new { sleeperId })).FirstOrDefault();
        }
    }
}
