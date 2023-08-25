using Football.Interfaces;
using Football.Models;
using Dapper;
using System.Data;

namespace Football.Services
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
    }
}
