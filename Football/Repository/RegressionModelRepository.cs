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
using System.Data;

namespace Football.Repository
{
    public class RegressionModelRepository : IRegressionModelRepository
    {
        private readonly string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = Football; Integrated Security=true;";
        public readonly ISqlQueryService _sqlQueryService;
        public readonly IDbConnection _dbConnection;

        public RegressionModelRepository(ISqlQueryService sqlQueryService, IDbConnection dbConnection)
        {
            _sqlQueryService = sqlQueryService;
            _dbConnection = dbConnection;
        }
        public PassingStatistic GetPassingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetPassingStatistic();
            return _dbConnection.Query<PassingStatistic>(query, new {season, playerId}).ToList().FirstOrDefault();
        }

        public RushingStatistic GetRushingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetRushingStatistic();
            return _dbConnection.Query<RushingStatistic>(query, new { season, playerId }).ToList().FirstOrDefault();
        }

        public ReceivingStatistic GetReceivingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetReceivingStatistic();
            return _dbConnection.Query<ReceivingStatistic>(query, new {season, playerId}).ToList().FirstOrDefault();
        }
    }
}
