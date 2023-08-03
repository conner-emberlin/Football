using CsvHelper;
using DataUpload.Classes;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataUpload.Uploads
{
    public class StatsUpload
    {
        private readonly string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";

        public List<PassingStats> PassingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            {
                return csv.GetRecords<PassingStats>().ToList();
            }

        }

        public List<ReceivingStats> ReceivingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            {
                return csv.GetRecords<ReceivingStats>().ToList();
            }

        }

        public List<RushingStats> RushingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            {
                return csv.GetRecords<RushingStats>().ToList();
            }

        }

        public async Task<int> PassingStatInsert(List<PassingStats> passingStats)
        {
            string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
            int count = new();
            using (var con = new SqlConnection(connection))
            {
                foreach (var pass in passingStats)
                {
                    var playerId = GetPlayerId(pass.PlayerName);
                    var year = 2018;

                    var query = $@"
                        INSERT INTO [dbo].PassingStats (Season, PlayerId, PassingYards, PassingAttempts, PassingCompletions, PassingTouchdowns, Interceptions) 
                        VALUES(@year, @playerid, @passingyards, @passingattempts, @passingcompletions, @passingtouchdowns, @interceptions)                   
                                 ";
                    if (playerId != 0)
                    {
                        count += await con.ExecuteAsync(query,
                            new
                            {
                                year,
                                playerId,
                                pass.PassingYards,
                                pass.PassingAttempts,
                                pass.PassingCompletions,
                                pass.PassingTouchdowns,
                                pass.Interceptions
                            });
                    }

                }

            }

            return count;

        }

        public async Task<int> ReceivingStatInsert(List<ReceivingStats> receivingStats)
        {
            string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
            int count = new();
            using (var con = new SqlConnection(connection))
            {
                foreach (var reception in receivingStats)
                {
                    var playerId = GetPlayerId(reception.PlayerName);
                    var year = 2018;

                    var query = $@"
                        INSERT INTO [dbo].ReceivingStats (Season, PlayerId, Receptions, ReceivingYards, Touchdowns, Targets) 
                        VALUES(@year, @playerid, @receptions, @yards, @tds, @tgts)                   
                                 ";
                    if (playerId != 0)
                    {
                        count += await con.ExecuteAsync(query,
                            new
                            {
                                year,
                                playerId,
                                receptions = reception.Receptions,
                                yards = reception.ReceivingYards,
                                tds = reception.Touchdowns,
                                tgts = reception.Targets                    
                            });
                    }

                }

            }

            return count;

        }

        public async Task<int> RushingStatInsert(List<RushingStats> rushingStats)
        {
            string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
            int count = new();
            using (var con = new SqlConnection(connection))
            {
                foreach (var rush in rushingStats)
                {
                    var playerId = GetPlayerId(rush.PlayerName);
                    var year = 2018;

                    var query = $@"
                        INSERT INTO [dbo].RushingStats (Season, PlayerId, RushingYards, RushingAttempts, Touchdowns, FirstDowns, Fumbles) 
                        VALUES(@year, @playerid, @yds, @att, @tds, @fds, @fum)                   
                                 ";
                    if (playerId != 0)
                    {
                        count += await con.ExecuteAsync(query,
                            new
                            {
                                year,
                                playerId,
                                yds = rush.RushingYards,
                                att = rush.RushingAttempts,
                                tds = rush.Touchdowns,
                                fds = rush.FirstDowns,
                                fum = rush.Fumbles
          
                            });
                    }

                }

            }

            return count;

        }

        public int GetPlayerId(string playerName)
        {
            var query = $@"
                    
                SELECT [Id]
                FROM [dbo].[Players]
                WHERE [Name] LIKE '%' + @name + '%'
                ";

            using (var con = new SqlConnection(connection))
            {
                return con.Query<int>(query, new { name = playerName }).ToList().FirstOrDefault();

            }

        }


    }
}
