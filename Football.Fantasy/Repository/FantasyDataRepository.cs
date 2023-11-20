using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using System.Data;
using Dapper;

namespace Football.Fantasy.Repository
{
    public class FantasyDataRepository(IDbConnection dbConnection) : IFantasyDataRepository
    {
        public async Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Season], [Games], [FantasyPoints], [Name], [Position]
                            FROM [dbo].SeasonFantasyData
                            WHERE [PlayerId] = @playerId";
            return (await dbConnection.QueryAsync<SeasonFantasy>(query, new { playerId })).ToList();
        }
        public async Task<int> PostSeasonFantasy(List<SeasonFantasy> data)
        {
            var query = $@"INSERT INTO [dbo].SeasonFantasyData (PlayerId, Season, Games, FantasyPoints, Name, Position)
                            VALUES (@PlayerId, @Season, @Games, @FantasyPoints, @Name, @Position)";
            return await dbConnection.ExecuteAsync(query, data);
        }
        public async Task<int> PostWeeklyFantasy(List<WeeklyFantasy> data)
        {
            var query = $@"INSERT INTO [dbo].WeeklyFantasyData (PlayerId, Name, Position, Season, Week, Games, FantasyPoints)
                        VALUES (@PlayerId, @Name, @Position, @Season, @Week, @Games, @FantasyPoints)";
            return await dbConnection.ExecuteAsync(query, data);
        }
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Season], [Week], [Games], [FantasyPoints]
                        FROM [dbo].WeeklyFantasyData
                        WHERE [PlayerId] = @playerId";
            return (await dbConnection.QueryAsync<WeeklyFantasy>(query, new {playerId})).ToList();
        }
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Season], [Week], [Games], [FantasyPoints]
                        FROM [dbo].WeeklyFantasyData
                        WHERE [Season] = @season
                                AND [Week] = @week";
            return (await dbConnection.QueryAsync<WeeklyFantasy>(query, new { season, week })).ToList();
        }
    }
}
