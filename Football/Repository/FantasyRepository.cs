using Football.Models;
using Football.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace Football.Repository
{
    public class FantasyRepository
    {
        private readonly string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = Football; Integrated Security=true;";
        public FantasyPassing GetFantasyPassing(int playerId, int season)
        {
            SqlQueryService sql = new();
            var query = sql.FantasyPassingQuery();
            using var con = new SqlConnection(connection);
            return con.Query<FantasyPassing>(query, new { playerId, season }).ToList().FirstOrDefault();        
        }

        public FantasyRushing GetFantasyRushing(int playerId, int season)
        {
            SqlQueryService sql = new();
            var query = sql.FantasyRushingQuery();
            using var con = new SqlConnection(connection);
            return con.Query<FantasyRushing>(query, new {playerId, season}).ToList().FirstOrDefault();
        }

        public FantasyReceiving  GetFantasyReceiving(int playerId, int season)
        {
            SqlQueryService sql = new();
            var query = sql.FantasyReceivingQuery();
            using var con = new SqlConnection(connection);
            return con.Query<FantasyReceiving>(query, new { playerId, season }).ToList().FirstOrDefault();
        }

        public List<int> GetPlayers()
        {
            SqlQueryService sql = new();
            var query =sql.GetPlayerIds();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query).ToList();
        }

        public string GetPlayerPosition(int playerId)
        {
            SqlQueryService sql = new();
            var query = sql.GetPlayerPosition();
            using var con = new SqlConnection(connection);
            return con.Query<string>(query, new {playerId}).ToList().FirstOrDefault();
        }

        public int InsertFantasyPoints(FantasyPoints fantasyPoints)
        {
            SqlQueryService sql = new();
            var query = sql.InsertFantasyData();
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
            SqlQueryService sql = new();
            var query = sql.GetFantasyPoints();
            using var con = new SqlConnection(connection);
            return con.Query<FantasyPoints>(query, new {playerId, season}).ToList().FirstOrDefault();
        }

        public List<int> GetPlayerIdsByFantasySeason(int season)
        {
            SqlQueryService sql = new();
            var query = sql.GetPlayerIdsByFantasySeason();
            using var con = new SqlConnection(connection);
            return con.Query<int>(query, new {season}).ToList();
        }

        public (int,int) RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            SqlQueryService sql = new();
            var deleteQuery = sql.DeleteFantasyPoints();
            using var con = new SqlConnection(connection);
            int removed = 0;
            int added = 0;
            removed += con.Execute(deleteQuery, new { fantasyPoints.PlayerId, fantasyPoints.Season });
            added += InsertFantasyPoints(fantasyPoints);

            return (removed, added);
        }
    }
}
