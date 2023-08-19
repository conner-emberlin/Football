﻿using Football.Models;
using Dapper;
using Football.Interfaces;
using System.Data;

namespace Football.Repository 
{
    public class FantasyRepository : IFantasyRepository
    {
        public readonly ISqlQueryService _sqlQueryService;
        public readonly IDbConnection _dbConnection;
        private readonly int _currentSeason = 2023;

        public FantasyRepository(ISqlQueryService sqlQueryService, IDbConnection dbConnection)
        {
            _sqlQueryService = sqlQueryService;
            _dbConnection = dbConnection;
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
        public async Task<(int, int)> RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            var deleteQuery = _sqlQueryService.DeleteFantasyPoints();
            int removed = 0;
            int added = 0;
            removed += await _dbConnection.ExecuteAsync(deleteQuery, new { fantasyPoints.PlayerId, fantasyPoints.Season });
            added += await InsertFantasyPoints(fantasyPoints);

            return (removed, added);
        }
        public async Task<int> InsertFantasyProjections(int rank, ProjectionModel proj)
        {
            var query = _sqlQueryService.InsertFantasyProjections();
            int season = _currentSeason;
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
