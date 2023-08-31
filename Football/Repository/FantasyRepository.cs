using Football.Models;
using Dapper;
using Football.Interfaces;
using System.Data;
using Microsoft.Extensions.Options;

namespace Football.Repository 
{
    public class FantasyRepository : IFantasyRepository
    {
        private readonly ISqlQueryService _sqlQueryService;
        private readonly IDbConnection _dbConnection;
        private readonly Season _season;

        public FantasyRepository(ISqlQueryService sqlQueryService, IDbConnection dbConnection, IOptionsMonitor<Season> season)
        {
            _sqlQueryService = sqlQueryService;
            _dbConnection = dbConnection;
            _season = season.CurrentValue;
        }

        public async Task<FantasyPassing> GetFantasyPassing(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyPassingQuery();
            return await _dbConnection.QueryFirstOrDefaultAsync<FantasyPassing>(query, new { playerId, season });        
        }

        public async Task<FantasyRushing> GetFantasyRushing(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyRushingQuery();
            return await _dbConnection.QueryFirstOrDefaultAsync<FantasyRushing>(query, new { playerId, season });
        }

        public async Task<FantasyReceiving>  GetFantasyReceiving(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyReceivingQuery();
            return await _dbConnection.QueryFirstOrDefaultAsync<FantasyReceiving>(query, new { playerId, season });
        }

        public async Task<int> InsertFantasyPoints(FantasyPoints fantasyPoints)
        {
            var query = _sqlQueryService.InsertFantasyData();
            int count = 0;
            return count += await _dbConnection.ExecuteAsync(query, new
            {
                fantasyPoints.Season,
                fantasyPoints.PlayerId,
                fantasyPoints.TotalPoints,
                fantasyPoints.PassingPoints,
                fantasyPoints.RushingPoints,
                fantasyPoints.ReceivingPoints
            });
        }

        public async Task<FantasyPoints> GetFantasyResults(int playerId, int season)
        {
            var query = _sqlQueryService.GetFantasyPoints();
            return await _dbConnection.QueryFirstOrDefaultAsync<FantasyPoints>(query, new {playerId, season});
        }
        public async Task<List<FantasyPoints>> GetAllFantasyResults(int playerId)
        {
            var query = _sqlQueryService.GetAllFantasyResults();
            var results = await _dbConnection.QueryAsync<FantasyPoints>(query, new { playerId });
            return results.ToList();
        }
        public async Task<int> RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            var deleteQuery = _sqlQueryService.DeleteFantasyPoints();
            int removed = 0;
            int added = 0;
            removed += await _dbConnection.ExecuteAsync(deleteQuery, new { fantasyPoints.PlayerId, fantasyPoints.Season });
            added += await InsertFantasyPoints(fantasyPoints);

            return added - removed;
        }
        public async Task<int> InsertFantasyProjections(int rank, ProjectionModel proj)
        {
            var query = _sqlQueryService.InsertFantasyProjections();
            int season = _season.CurrentSeason;
            return await _dbConnection.ExecuteAsync(query, new
            {
                season,
                proj.PlayerId,
                proj.Name,
                proj.Position,
                rank,
                ProjectedPoints = Math.Round((double)proj.ProjectedPoints)
            });
        }



    }
}
