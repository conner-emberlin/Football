using Football.Interfaces;
using Football.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Football.Repository
{
    public class DataUploadRepository : IDataUploadRepository
    {
        public readonly ISqlQueryService _sqlQueryService;
        public readonly IDbConnection _connection;
        public DataUploadRepository(ISqlQueryService sqlQueryService, IDbConnection dbCoonnection) { 
            _sqlQueryService = sqlQueryService;
            _connection = dbCoonnection;
        }
        public async Task<int> PassingStatInsert(List<PassingStatistic> statistics, int season)
        {
            var query = _sqlQueryService.GetQueryString("passing");
            int count = 0;

            foreach (var stat in statistics)
            {
                count += await _connection.ExecuteAsync(query, new
                {
                    season,
                    stat.Name,
                    stat.Team,
                    stat.Age,
                    stat.Games,
                    stat.Completions,
                    stat.Attempts,
                    stat.Yards,
                    stat.Touchdowns,
                    stat.Interceptions,
                    stat.FirstDowns,
                    stat.Long,
                    stat.Sacks,
                    stat.SackYards
                });
            }
            return count;

        }

        public async Task<int> ReceivingStatInsert(List<ReceivingStatistic> statistics, int season)
        {
            var query = _sqlQueryService.GetQueryString("receiving");
            int count = 0;
            foreach (var stat in statistics)
            {
                count += await _connection.ExecuteAsync(query, new
                {
                    season,
                    stat.Name,
                    stat.Team,
                    stat.Age,
                    stat.Games,
                    stat.Targets,
                    stat.Receptions,
                    stat.Yards,
                    stat.Touchdowns,
                    stat.FirstDowns,
                    stat.Long,
                    stat.RpG,
                    stat.Fumbles
                });
            }
            return count;
        }

        public async Task<int> RushingStatInsert(List<RushingStatistic> statistics, int season)
        {
            var query = _sqlQueryService.GetQueryString("rushing");
            int count = 0;
            foreach (var stat in statistics)
            {
                count += await _connection.ExecuteAsync(query, new
                {
                    season,
                    stat.Name,
                    stat.Team,
                    stat.Age,
                    stat.Games,
                    stat.RushAttempts,
                    stat.Yards,
                    stat.Touchdowns,
                    stat.FirstDowns,
                    stat.Long,
                    stat.Fumbles
                });
            }
            return count;
        }
    }
}
