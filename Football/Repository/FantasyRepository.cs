using Football.Models;
using Football.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Football.Interfaces;
using System.Data;

namespace Football.Repository 
{
    public class FantasyRepository : IFantasyRepository
    {
        public readonly ISqlQueryService _sqlQueryService;
        public readonly IDbConnection _dbConnection;

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

        public async Task<List<int>> GetPlayers()
        {
            var query =_sqlQueryService.GetPlayerIds();
            var players = await _dbConnection.QueryAsync<int>(query);
            return players.ToList();
        }

        public async Task<string> GetPlayerPosition(int playerId)
        {
            var query = _sqlQueryService.GetPlayerPosition();
            return await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { playerId });
        }

        public async Task<List<int>> GetPlayersByPosition(string position)
        {
            var query = _sqlQueryService.GetPlayersByPosition();
            var players = await _dbConnection.QueryAsync<int>(query, new {position});
            return players.ToList();
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

        public async Task<List<int>> GetPlayerIdsByFantasySeason(int season)
        {
            var query = _sqlQueryService.GetPlayerIdsByFantasySeason();
            var players = await _dbConnection.QueryAsync<int>(query, new { season });
            return players.ToList();
        }

        public async Task<(int,int)> RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            var deleteQuery = _sqlQueryService.DeleteFantasyPoints();
            int removed = 0;
            int added = 0;
            removed += await _dbConnection.ExecuteAsync(deleteQuery, new { fantasyPoints.PlayerId, fantasyPoints.Season });
            added +=await  InsertFantasyPoints(fantasyPoints);

            return (removed, added);
        }

        public async Task<List<int>> GetActiveSeasons(int playerId)
        {
            var query = _sqlQueryService.GetSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new {playerId});
            return seasons.ToList();
        }

        public async Task<List<FantasySeasonGames>> GetAverageTotalGames(int playerId, string position)
        {
            string query;
            if(position == "QB")
            {
                query = _sqlQueryService.GetQbGames();
            }
            else if (position == "RB")
            {
                query = _sqlQueryService.GetRbGames();
            }
            else
            {
                query = _sqlQueryService.GetPcGames();
            }
            var games = await _dbConnection.QueryAsync<FantasySeasonGames>(query, new { playerId });
            return games.ToList();
        }

        public async Task<List<int>> GetActivePassingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActivePassingSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return seasons.ToList();
        }

        public async Task<List<int>> GetActiveRushingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveRushingSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return seasons.ToList();
        }

        public async Task<List<int>> GetActiveReceivingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveReceivingSeasons();
            var seasons = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return seasons.ToList();
        }

        public async Task<string> GetPlayerName(int playerId)
        {
            var query = _sqlQueryService.GetPlayerName();
            var players = await _dbConnection.QueryAsync<string>(query, new { playerId });
            return players.FirstOrDefault().ToString();
        }

        public async Task<bool> IsPlayerActive(int playerId)
        {
            var query = _sqlQueryService.IsPlayerActive();
            var truth = await _dbConnection.QueryAsync<int>(query, new { playerId });
            return truth.FirstOrDefault() == 1;
        }

        public async Task<string> GetPlayerTeam(int playerId)
        {
            var query = _sqlQueryService.GetPlayerTeam();
            var team = await _dbConnection.QueryAsync<string>(query, new { playerId });
            return team.FirstOrDefault().ToString();
        }

        public async Task<List<int>> GetTightEnds()
        {
            var query = _sqlQueryService.GetTightEnds();
            var tightEnds = await _dbConnection.QueryAsync<int>(query);
            return tightEnds.ToList();
        }
    }
}
