using Dapper;
using Football.LeagueAnalysis.Interfaces;
using Football.LeagueAnalysis.Models;
using System.Data;

namespace Football.LeagueAnalysis.Repository
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
    }
}
