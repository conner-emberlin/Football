using NFLData.Models;
using NFLData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;


namespace DataAnalysis.Repository
{
    public class RegModelRepository
    {
        private string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
        PlayerRepository playerRepo = new();
        
        public double GetPassingCompletionPercentage(Player player, int season)
        {
            var query = $@"
                    
                SELECT [CompletionPercentage]
                FROM [dbo].[PassingAnalysis]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());
        }

        public int GetRbRushAttempts(Player player, int season)
        {
            var query = $@"
                    
                SELECT [RushingAttempts]
                FROM [dbo].[RushingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public int GetFantasyPoints(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Points]
                FROM [dbo].[FantasyStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public int GetTargets(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Targets]
                FROM [dbo].[ReceivingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public List<Player> GetPlayersByPosition(string team, int season, string position)
        {
            var query = $@"
                    
                SELECT [Position], [Team], [Name]
                FROM [dbo].[Players] p
                WHERE 
                [Team] = @team             
                AND [Position] = @position
                AND (EXISTS(SELECT * FROM [dbo].RushingStats rs WHERE p.[Id] = rs.[PlayerId]
                            AND [Season] = @season)
                    OR EXISTS(SELECT * FROM [dbo].ReceivingStats res WHERE p.[Id] = res.[PlayerId])
                    OR EXISTS(SELECT * FROM [dbo].PassingStats ps WHERE p.[Id] = ps.[PlayerId]))                   
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<Player>(query, new { team, season, position }).ToList());
        }

    }
}
