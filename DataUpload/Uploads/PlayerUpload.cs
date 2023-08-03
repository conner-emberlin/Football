using CsvHelper;
using DataUpload.Classes;
using System.Data.SqlClient;
using Dapper;
using NFLData.Services;

namespace DataUpload.Uploads
{
    public class PlayerUpload
    {
        private static string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
        public List<Player> PlayerFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));            
            return csv.GetRecords<Player>().ToList();           
        }


        public List<PlayerSeason> PlayerSeasonUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            return csv.GetRecords<PlayerSeason>().ToList();
        }
        public async Task<int> PlayerInsert(List<Player> players)
        {           
            int count = new();
            using var con = new SqlConnection(connection);            
            foreach (var player in players)
                {
                    var query = $@"
                        INSERT INTO [dbo].Players (Position, Team, Name) 
                        VALUES(@position, @team, @name)                   
                ";
                    count += await con.ExecuteAsync(query, new { player.Position, player.Team, player.Name });
                }
            
            return count;
        }

        public async Task<int> PlayerSeasonInsert(List<PlayerSeason> seasons)
        {
            int count = new();
            int year = 2018;
            PlayerInfo pf = new();
            using var con = new SqlConnection(connection);
            foreach(var season in seasons)
            {
                if (pf.GetPlayerId(season.Name) != 0) {
                    var query = $@"
                        INSERT INTO [dbo].PlayerSeason (Season, PlayerId, Age, Games)
                        VALUES(@season, @playerid, @age, @games)
                        ";
                    count += await con.ExecuteAsync(query, new {

                        Season = year,
                        playerid = pf.GetPlayerId(season.Name),
                        age = season.Age,                 
                        games = season.Games
                    }); }

            }
            return count;

        }
    }
}
