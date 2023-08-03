using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Football.Models;

namespace Football.Services
{
    internal class DataUploadService
    {
        private readonly string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = Football; Integrated Security=true;";
            public List<PassingStatistic> PassingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            return csv.GetRecords<PassingStatistic>().ToList();
        }

        public List<ReceivingStatistic> ReceivingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            return csv.GetRecords<ReceivingStatistic>().ToList();
        }

        public List<RushingStatistic> RushingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            return csv.GetRecords<RushingStatistic>().ToList();
        }
        public int PassingStatInsert(List<PassingStatistic> statistics, int season)
        {
            SqlQueryService sql = new();
            using var con = new SqlConnection(connection);
            var query = sql.GetQueryString("passing");
            
            int count = 0;

            foreach (var stat in statistics)
            {
                count += con.Execute(query, new
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

        public int ReceivingStatInsert(List<ReceivingStatistic> statistics, int season)
        {
            SqlQueryService sql = new();
            using var con = new SqlConnection(connection);
            var query = sql.GetQueryString("receiving");

            int count = 0;

            foreach (var stat in statistics)
            {
                count += con.Execute(query, new
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

        public int RushingStatInsert(List<RushingStatistic> statistics, int season)
        {
            SqlQueryService sql = new();
            using var con = new SqlConnection(connection);
            var query = sql.GetQueryString("rushing");

            int count = 0;

            foreach (var stat in statistics)
            {
                count += con.Execute(query, new
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

