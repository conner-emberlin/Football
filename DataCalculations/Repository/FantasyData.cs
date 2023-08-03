using NFLData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFLData.Repositories;
using System.Data.SqlClient;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DataCalculations.Models;

namespace DataCalculations.Repository
{
    public class FantasyData
    {
        private string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
        PlayerRepository playerRepo = new PlayerRepository();
        public List<int> GetPlayerSeasons(Player player)
        {
            var id = playerRepo.GetPlayerId(player.Name);
            if (player.Position == "QB")
            {
                var query = $@"
                    
                SELECT DISTINCT [Season]
                FROM [dbo].[PassingStats]
                WHERE [PlayerId] = @id
                ";

                using var con = new SqlConnection(connection);
                return (con.Query<int>(query, new { id }).ToList());

            }

            if (player.Position == "RB")
            {
                var query = $@"
                    
                SELECT DISTINCT [Season]
                FROM [dbo].[RushingStats]
                WHERE [PlayerId] = @id
                ";

                using var con = new SqlConnection(connection);
                return (con.Query<int>(query, new { id }).ToList());

            }

            else
            {
                var query = $@"
                    
                SELECT DISTINCT [Season]
                FROM [dbo].[ReceivingStats]
                WHERE [PlayerId] = @id
                ";

                using var con = new SqlConnection(connection);
                return (con.Query<int>(query, new { id }).ToList());

            }


        }

        public int GetPassingYards(Player player, int season)
        {
            var query = $@"
                    
                SELECT [PassingYards]
                FROM [dbo].[PassingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());


        }

        public int GetPassingAttempts(Player player, int season)
        {
            var query = $@"
                    
                SELECT [PassingAttempts]
                FROM [dbo].[PassingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public int GetPassingCompletions(Player player, int season)
        {
            var query = $@"
                    
                SELECT [PassingCompletions]
                FROM [dbo].[PassingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public int GetPassingTouchdowns(Player player, int season)
        {
            var query = $@"
                    
                SELECT [PassingTouchdowns]
                FROM [dbo].[PassingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());


        }

        public int GetRushingYards(Player player, int season)
        {
            var query = $@"
                    
                SELECT [RushingYards]
                FROM [dbo].[RushingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());


        }

        public int GetRushingTouchdowns(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Touchdowns]
                FROM [dbo].[RushingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());


        }

        public int GetFumbles(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Fumbles]
                FROM [dbo].[RushingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());


        }

        public int GetReceptions(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Receptions]
                FROM [dbo].[ReceivingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());


        }

        public int GetReceivingYards(Player player, int season)
        {
            var query = $@"
                    
                SELECT [ReceivingYards]
                FROM [dbo].[ReceivingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public int GetReceivingTouchdowns(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Touchdowns]
                FROM [dbo].[ReceivingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public int GetInterceptions(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Interceptions]
                FROM [dbo].[PassingStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";

            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());

        }

        public int PublishFantasyResults(FantasySeasonResults result)
        {
            int count = new();
            using var con = new SqlConnection(connection);
                var query = $@"
                        INSERT INTO [dbo].FantasyStats (Season, Format, PlayerId, Points) 
                        VALUES(@season, @format, @playerid, @points)                   
                                 ";
                count +=  con.Execute(query,
                    new
                    {
                        result.Season,
                        result.Format,
                        result.PlayerId,
                        result.Points
                    });            
            return count;
        }

        public int GetFantasyPoints(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Points]
                FROM [dbo].[FantasyStats]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                AND [Format] = 6
                ";
            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());
        }

        public int GetGamesPlayed(Player player, int season)
        {
            var query = $@"
                    
                SELECT [Games]
                FROM [dbo].[PlayerSeason]
                WHERE [PlayerId] = @id
                AND [Season] = @season
                ";
            using var con = new SqlConnection(connection);
            return (con.Query<int>(query, new { id = playerRepo.GetPlayerId(player.Name), season }).ToList().FirstOrDefault());


        }

    }
}
