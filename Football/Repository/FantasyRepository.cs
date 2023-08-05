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

        public FantasyPassing GetFantasyPassing(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyPassingQuery();
            return _dbConnection.Query<FantasyPassing>(query, new { playerId, season }).ToList().FirstOrDefault();        
        }

        public FantasyRushing GetFantasyRushing(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyRushingQuery();
            return _dbConnection.Query<FantasyRushing>(query, new {playerId, season}).ToList().FirstOrDefault();
        }

        public FantasyReceiving  GetFantasyReceiving(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyReceivingQuery();
            return _dbConnection.Query<FantasyReceiving>(query, new { playerId, season }).ToList().FirstOrDefault();
        }

        public List<int> GetPlayers()
        {
            var query =_sqlQueryService.GetPlayerIds();
            return _dbConnection.Query<int>(query).ToList();
        }

        public string GetPlayerPosition(int playerId)
        {
            var query = _sqlQueryService.GetPlayerPosition();
            return _dbConnection.Query<string>(query, new {playerId}).ToList().FirstOrDefault();
        }

        public List<int> GetPlayersByPosition(string position)
        {
            var query = _sqlQueryService.GetPlayersByPosition();
            return _dbConnection.Query<int>(query, new {position}).ToList();
        }

        public int InsertFantasyPoints(FantasyPoints fantasyPoints)
        {
            var query = _sqlQueryService.InsertFantasyData();
            int count = 0;
            return count += _dbConnection.Execute(query, new
            {
                fantasyPoints.Season,
                fantasyPoints.PlayerId,
                fantasyPoints.TotalPoints,
                fantasyPoints.PassingPoints,
                fantasyPoints.RushingPoints,
                fantasyPoints.ReceivingPoints
            });
        }

        public FantasyPoints GetFantasyResults(int playerId, int season)
        {
            var query = _sqlQueryService.GetFantasyPoints();
            return _dbConnection.Query<FantasyPoints>(query, new {playerId, season}).ToList().FirstOrDefault();
        }

        public List<int> GetPlayerIdsByFantasySeason(int season)
        {
            var query = _sqlQueryService.GetPlayerIdsByFantasySeason();
            return _dbConnection.Query<int>(query, new {season}).ToList();
        }

        public (int,int) RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            var deleteQuery = _sqlQueryService.DeleteFantasyPoints();
            int removed = 0;
            int added = 0;
            removed += _dbConnection.Execute(deleteQuery, new { fantasyPoints.PlayerId, fantasyPoints.Season });
            added += InsertFantasyPoints(fantasyPoints);

            return (removed, added);
        }

        public List<int> GetActiveSeasons(int playerId)
        {
            var query = _sqlQueryService.GetSeasons();
            return _dbConnection.Query<int>(query, new {playerId}).ToList();
        }

        public double GetAverageTotalGames(int playerId, string position)
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
            return _dbConnection.Query<int>(query, new {playerId}).DefaultIfEmpty(0).Average();
        }

        public List<int> GetActivePassingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActivePassingSeasons();
            return _dbConnection.Query<int>(query, new { playerId }).ToList();
        }

        public List<int> GetActiveRushingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveRushingSeasons();
            return _dbConnection.Query<int>(query, new { playerId }).ToList();
        }

        public List<int> GetActiveReceivingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveReceivingSeasons();
            return _dbConnection.Query<int>(query, new { playerId }).ToList();
        }

        public string GetPlayerName(int playerId)
        {
            var query = _sqlQueryService.GetPlayerName();
            return _dbConnection.Query<string>(query, new { playerId }).FirstOrDefault().ToString();
        }

        public bool IsPlayerActive(int playerId)
        {
            var query = _sqlQueryService.IsPlayerActive();
            return _dbConnection.Query<int>(query, new {playerId}).FirstOrDefault() == 1;
        }

        public string GetPlayerTeam(int playerId)
        {
            var query = _sqlQueryService.GetPlayerTeam();
            return _dbConnection.Query<string>(query, new {playerId}).FirstOrDefault().ToString();
        }
    }
}
