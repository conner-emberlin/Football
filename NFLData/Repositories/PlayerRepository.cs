using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NFLData.Models;
using System.Data.SqlClient;


namespace NFLData.Repositories
{
    public class PlayerRepository
    {
        string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
        public List<Player> GetPlayersByName(string playerName)
        {            
            var query = $@"
                    
                SELECT [Id]
                    ,[Position]
                    ,[Team]
                    ,[Name]
                FROM [dbo].[Players]
                WHERE [Name] LIKE '%' + @name + '%'
                ";

            using (var con = new SqlConnection(connection))
            {
                return (con.Query<Player>(query, new {name = playerName}).ToList());

            }

        }


        public List<Player> GetPlayersByPosition(string playerPosition)
        {
            var query = $@"
                    
                SELECT [Id]
                    ,[Position]
                    ,[Team]
                    ,[Name]
                FROM [dbo].[Players]
                WHERE [Position] LIKE '%' + @position + '%'
                ";

            using (var con = new SqlConnection(connection))
            {
                return (con.Query<Player>(query, new {position = playerPosition }).ToList());

            }

        }


        public List<Player> GetPlayersByTeam(string playerTeam)
        {
            var query = $@"
                    
                SELECT [Id]
                    ,[Position]
                    ,[Team]
                    ,[Name]
                FROM [dbo].[Players]
                WHERE [Team] LIKE '%' + @team + '%'
                ";

            using (var con = new SqlConnection(connection))
            {
                return (con.Query<Player>(query, new {team = playerTeam}).ToList());

            }

        }

        public List<Player> GetPlayers()
        {

            var query = $@"
                    
                SELECT 
                    [Position]
                    ,[Team]
                    ,[Name]
                FROM [dbo].[Players]
                ";

            using (var con = new SqlConnection(connection))
            {
                return (con.Query<Player>(query).ToList());

            }

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
                return (con.Query<int>(query, new { name = playerName }).ToList().FirstOrDefault());

            }

        }

    }
}
