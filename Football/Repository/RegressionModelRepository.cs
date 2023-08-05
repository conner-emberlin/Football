using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using Football.Services;
using System.Data.SqlClient;
using Dapper;
using Football.Interfaces;

namespace Football.Repository
{
    public class RegressionModelRepository : IRegressionModelRepository
    {
        private readonly string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = Football; Integrated Security=true;";
        public readonly ISqlQueryService _sqlQueryService;

        public RegressionModelRepository(ISqlQueryService sqlQueryService)
        {
            _sqlQueryService = sqlQueryService;
        }
        public PassingStatistic GetPassingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetPassingStatistic();
            using var con = new SqlConnection(connection);
            return con.Query<PassingStatistic>(query, new {season, playerId}).ToList().FirstOrDefault();
        }

        public RushingStatistic GetRushingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetRushingStatistic();
            using var con = new SqlConnection(connection);
            return con.Query<RushingStatistic>(query, new { season, playerId }).ToList().FirstOrDefault();
        }

        public ReceivingStatistic GetReceivingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetReceivingStatistic();
            using var con = new SqlConnection(connection);
            return con.Query<ReceivingStatistic>(query, new {season, playerId}).ToList().FirstOrDefault();
        }
    }
}
