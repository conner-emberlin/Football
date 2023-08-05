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

namespace Football.Repository 
{
    public class FantasyRepository : IFantasyRepository
    {
        private readonly string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = Football; Integrated Security=true;";
        public readonly ISqlQueryService _sqlQueryService;

        public FantasyRepository(ISqlQueryService sqlQueryService)
        {
            _sqlQueryService = sqlQueryService;
        }

        public FantasyPassing GetFantasyPassing(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyPassingQuery();
            using var con = new SqlConnection(connection);
            return con.Query<FantasyPassing>(query, new { playerId, season }).ToList().FirstOrDefault();        
        }

        public FantasyRushing GetFantasyRushing(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyRushingQuery();
            using var con = new SqlConnection(connection);
            return con.Query<FantasyRushing>(query, new {playerId, season}).ToList().FirstOrDefault();
        }

        public FantasyReceiving  GetFantasyReceiving(int playerId, int season)
        {
            var query = _sqlQueryService.FantasyReceivingQuery();
            using var con = new SqlConnection(connection);
            return con.Query<FantasyReceiving>(query, new { playerId, season }).ToList().FirstOrDefault();
        }

        public List<int> GetPlayers()
        {
            var query =_sqlQueryService.GetPlayerIds();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query).ToList();
        }

        public string GetPlayerPosition(int playerId)
        {
            var query = _sqlQueryService.GetPlayerPosition();
            using var con = new SqlConnection(connection);
            return con.Query<string>(query, new {playerId}).ToList().FirstOrDefault();
        }

        public List<int> GetPlayersByPosition(string position)
        {
            var query = _sqlQueryService.GetPlayersByPosition();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new {position}).ToList();
        }

        public int InsertFantasyPoints(FantasyPoints fantasyPoints)
        {
            var query = _sqlQueryService.InsertFantasyData();
            int count = 0;
            using var con = new SqlConnection(connection);
            return count += con.Execute(query, new
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
            using var con = new SqlConnection(connection);
            return con.Query<FantasyPoints>(query, new {playerId, season}).ToList().FirstOrDefault();
        }

        public List<int> GetPlayerIdsByFantasySeason(int season)
        {
            var query = _sqlQueryService.GetPlayerIdsByFantasySeason();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new {season}).ToList();
        }

        public (int,int) RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            var deleteQuery = _sqlQueryService.DeleteFantasyPoints();
            using var con = new SqlConnection(connection);
            int removed = 0;
            int added = 0;
            removed += con.Execute(deleteQuery, new { fantasyPoints.PlayerId, fantasyPoints.Season });
            added += InsertFantasyPoints(fantasyPoints);

            return (removed, added);
        }

        public List<int> GetActiveSeasons(int playerId)
        {
            var query = _sqlQueryService.GetSeasons();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new {playerId}).ToList();
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
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new {playerId}).DefaultIfEmpty(0).Average();
        }

        public List<int> GetActivePassingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActivePassingSeasons();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new { playerId }).ToList();
        }

        public List<int> GetActiveRushingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveRushingSeasons();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new { playerId }).ToList();
        }

        public List<int> GetActiveReceivingSeasons(int playerId)
        {
            var query = _sqlQueryService.GetActiveReceivingSeasons();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new { playerId }).ToList();
        }

        public string GetPlayerName(int playerId)
        {
            var query = _sqlQueryService.GetPlayerName();
            using var con = new SqlConnection(connection);
            return con.Query<string>(query, new { playerId }).FirstOrDefault().ToString();
        }

        public bool IsPlayerActive(int playerId)
        {
            var query = _sqlQueryService.IsPlayerActive();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new {playerId}).FirstOrDefault() == 1;
        }

        public string GetPlayerTeam(int playerId)
        {
            var query = _sqlQueryService.GetPlayerTeam();
            using var con = new SqlConnection(connection);
            return con.Query<string>(query, new {playerId}).FirstOrDefault().ToString();
        }
    }
}
