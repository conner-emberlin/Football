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
        public readonly ISqlQueryService _sqlQueryService;
        public readonly IDbConnection _dbConnection;

        public RegressionModelRepository(ISqlQueryService sqlQueryService, IDbConnection dbConnection)
        {
            _sqlQueryService = sqlQueryService;
            _dbConnection = dbConnection;
        }
        public async Task<PassingStatistic> GetPassingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetPassingStatistic();
            return await _dbConnection.QueryFirstOrDefaultAsync<PassingStatistic>(query, new {season, playerId});
        }

        public async Task<PassingStatisticWithSeason> GetPassingStatisticWithSeason(int playerId, int season)
        {
            var query = _sqlQueryService.GetPassingStatisticWithSeason();
            return await _dbConnection.QueryFirstOrDefaultAsync<PassingStatisticWithSeason>(query, new {season, playerId});
        }

        public async Task<RushingStatistic> GetRushingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetRushingStatistic();
            return await _dbConnection.QueryFirstOrDefaultAsync<RushingStatistic>(query, new { season, playerId });
        }

        public async Task<RushingStatisticWithSeason> GetRushingStatisticWithSeason(int playerId, int season)
        {
            var query = _sqlQueryService.GetRushingStatisticWithSeason();
            return await _dbConnection.QueryFirstOrDefaultAsync<RushingStatisticWithSeason>(query, new { season, playerId });
        }

        public async Task<ReceivingStatistic> GetReceivingStatistic(int playerId, int season)
        {
            var query = _sqlQueryService.GetReceivingStatistic();
            return await _dbConnection.QueryFirstOrDefaultAsync<ReceivingStatistic>(query, new { season, playerId }); ;
        }

        public async Task<ReceivingStatisticWithSeason> GetReceivingStatisticWithSeason(int playerId, int season)
        {
            var query = _sqlQueryService.GetReceivingStatisticWithSeason();
            return await _dbConnection.QueryFirstOrDefaultAsync<ReceivingStatisticWithSeason>(query, new { season, playerId });
        }
    }
}
