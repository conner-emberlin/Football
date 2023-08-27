using Football.Interfaces;
using Football.Models;
using Dapper;
using System.Data;

namespace Football.Repository
{
    public class AdjustmentRepository : IAdjustmentRepository
    {
        private readonly ISqlQueryService _sqlQueryService;
        private readonly IDbConnection _dbConnection;

        public AdjustmentRepository(ISqlQueryService sqlQueryService, IDbConnection dbConnection)
        {
            _sqlQueryService = sqlQueryService;
            _dbConnection = dbConnection;
        }
        public async Task<int> GetGamesSuspended(int playerId, int season)
        {
            var query = _sqlQueryService.GetGamesSuspended();
            var sus = await _dbConnection.QueryAsync<int>(query, new { playerId, season });
            return sus.FirstOrDefault();
        }
        public async Task<List<Change>> GetTeamChange(int season, string team)
        {
            var query = _sqlQueryService.GetTeamChange();
            var changes = await _dbConnection.QueryAsync<Change>(query, new { season, team });
            return changes.ToList();
        }
        public async Task<Change> GetTeamChange(int season, int playerId)
        {
            var query = _sqlQueryService.GetPlayerTeamChange();
            var changes = await _dbConnection.QueryAsync<Change>(query, new { season, playerId });
            return changes.FirstOrDefault();
        }
    }
}
