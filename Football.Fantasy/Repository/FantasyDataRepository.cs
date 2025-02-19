﻿using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using System.Data;
using Dapper;
using Football.Enums;

namespace Football.Fantasy.Repository
{
    public class FantasyDataRepository(IDbConnection dbConnection) : IFantasyDataRepository
    {
        public async Task<List<SeasonFantasy>> GetSeasonFantasy(int filter, bool isPlayer = true)
        {
            var query = $@"SELECT [PlayerId], [Season], [Games], [FantasyPoints], [Name], [Position]
                            FROM [dbo].SeasonFantasyData";
            query += isPlayer ? " WHERE [PlayerId] = @filter" : " WHERE [Season] = @filter";                
            return (await dbConnection.QueryAsync<SeasonFantasy>(query, new { filter })).ToList();
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
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasyByPlayer(int playerId, int season)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Season], [Week], [Games], [FantasyPoints]
                        FROM [dbo].WeeklyFantasyData
                        WHERE [PlayerId] = @playerId
                            AND [Season] = @season";
            return (await dbConnection.QueryAsync<WeeklyFantasy>(query, new {playerId, season})).ToList();
        }
        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Season], [Week], [Games], [FantasyPoints]
                        FROM [dbo].WeeklyFantasyData
                        WHERE [Season] = @season";
            if (week > 0) query += " AND [Week] = @week";
            return (await dbConnection.QueryAsync<WeeklyFantasy>(query, new { season, week })).ToList();
        }

        public async Task<List<WeeklyFantasy>> GetAllWeeklyFantasyByPosition(string position, int season = 0)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyFantasyData 
                            WHERE [Position] = @position";
            query += season > 0 ? " AND [Season] = @season" : "";
            query += " ORDER BY [PlayerId], [Season], [Week]";
            return (await dbConnection.QueryAsync<WeeklyFantasy>(query, new { position, season})).ToList();
        }

        public async Task<IEnumerable<WeeklyFantasy>> GetAllWeeklyFantasy(int season)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyFantasyData WHERE [Season] = @season";
            return await dbConnection.QueryAsync<WeeklyFantasy>(query, new { season });
        }
        public async Task<List<SeasonFantasy>> GetAllSeasonFantasyByPosition(string position, int minGames)
        {
            var query = $@"SELECT * FROM [dbo].SeasonFantasyData 
                            WHERE [Position] = @position AND [Games] >= @minGames
                            ORDER BY [PlayerId], [Season]";
            return (await dbConnection.QueryAsync<SeasonFantasy>(query, new { position, minGames })).ToList();
        }

        public async Task<Dictionary<int, double>> GetAverageWeeklyFantasyPoints(IEnumerable<int> playerIds, int season)
        {
            var query = $@"select PlayerId, AVG(FantasyPoints) as AverageFantasy
                           from WeeklyFantasyData 
                           where Season = @season
                           and PlayerId IN @playerIds
                           group by PlayerId";
            return (await dbConnection.QueryAsync<(int PlayerId, double AverageFantasy)>(query, new { playerIds, season })).ToDictionary(p => p.PlayerId, p => p.AverageFantasy);
        }
    }
}
