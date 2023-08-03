using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using Football.Services;
using System.Data.SqlClient;
using Dapper;

namespace Football.Repository
{
    public class RegressionModelRepository
    {
        private readonly string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = Football; Integrated Security=true;";
        public PassingStatistic GetPassingStatistic(int playerId, int season)
        {
            SqlQueryService sql = new();
            var query = sql.GetPassingStatistic();
            using var con = new SqlConnection(connection);
            return con.Query<PassingStatistic>(query, new {season, playerId}).ToList().FirstOrDefault();
        }

        public RushingStatistic GetRushingStatistic(int playerId, int season)
        {
            SqlQueryService sql = new();
            var query = sql.GetRushingStatistic();
            using var con = new SqlConnection(connection);
            return con.Query<RushingStatistic>(query, new { season, playerId }).ToList().FirstOrDefault();
        }

        public ReceivingStatistic GetReceivingStatistic(int playerId, int season)
        {
            SqlQueryService sql = new();
            var query = sql.GetReceivingStatistic();
            using var con = new SqlConnection(connection);
            return con.Query<ReceivingStatistic>(query, new {season, playerId}).ToList().FirstOrDefault();
        }
    }
}
