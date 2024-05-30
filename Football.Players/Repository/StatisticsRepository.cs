﻿using Football.Players.Interfaces;
using Football.Players.Models;
using System.Data;
using Dapper;
using Football.Enums;

namespace Football.Players.Repository
{
    public class StatisticsRepository(IDbConnection dbConnection) : IStatisticsRepository
    {
        public async Task<List<T>> GetWeeklyData<T>(Position position, int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].[{GetWeeklyTable(position)}] WHERE [Season] = @season";
            if (week > 0) query += " AND [Week] = @week";
            return (await dbConnection.QueryAsync<T>(query, new { season, week })).ToList();
        }
        public async Task<List<T>> GetWeeklyDataByPlayer<T>(Position position, int playerId, int season)
        {
            var query = $@"SELECT * FROM [dbo].[{GetWeeklyTable(position)}] WHERE [PlayerId] = @playerId AND [Season] = @season";
            return (await dbConnection.QueryAsync<T>(query, new { playerId, season })).ToList();
        }

        public async Task<List<T>> GetAllWeeklyDataByPosition<T>(Position position)
        {
            var query = "";

            if (position == Position.DST || position == Position.K)
            {
                query = $@"SELECT * FROM [dbo].[{GetWeeklyTable(position)}] w";

            }
            else
            {
                query = $@" SELECT w.*, COALESCE(s.Snaps, 0) AS Snaps
                            FROM [dbo].[{GetWeeklyTable(position)}] w
                            LEFT OUTER JOIN SnapCount s
                            ON w.PlayerId = s.PlayerId
                            AND w.Season = s.Season
                            AND w.Week = s.Week
                            ORDER BY w.[PlayerId], w.[Season], w.[Week]";
            }   

            return (await dbConnection.QueryAsync<T>(query)).ToList();
        }

        public async Task<List<T>> GetAllSeasonDataByPosition<T>(Position position, int minGames)
        {
            var query = $@"SELECT * FROM [dbo].[{GetSeasonTable(position)}]
                            WHERE [Games] > @minGames
                            ORDER BY [PlayerId], [Season]";
            return (await dbConnection.QueryAsync<T>(query, new {minGames})).ToList();
        }

        public async Task<List<T>> GetSeasonData<T>(Position position, int queryParam, bool isPlayer)
        {
            var query = $@"SELECT * FROM [dbo].[{GetSeasonTable(position)}] WHERE {GetSeasonQueryWhere(isPlayer)}";
            return (await dbConnection.QueryAsync<T>(query, new { queryParam })).ToList();
        }

        public async Task<List<GameResult>> GetGameResults(int season)
        {
            var query = $@"SELECT * FROM [dbo].GameResults WHERE [Season] = @season";                                               
            return (await dbConnection.QueryAsync<GameResult>(query, new { season})).ToList();
        }

        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyRosterPercentages
                            WHERE [Season] = @season
                                AND [Week] = @week";
            return (await dbConnection.QueryAsync<WeeklyRosterPercent>(query, new { season, week })).ToList();
        }
        public async Task<List<SnapCount>> GetSnapCounts(int playerId, int season)
        {
            var query = $@"SELECT * FROM [dbo].SnapCount WHERE [PlayerId] = @playerId AND [Season] = @season";
            return (await dbConnection.QueryAsync<SnapCount>(query, new {playerId, season})).ToList();
        }

        public async Task<double> GetSnapsByGame(int playerId, int season, int week)
        {
            var query = $@"SELECT [Snaps] FROM [dbo].SnapCount WHERE [PlayerId] = @playerId AND [Season] = @season AND [Week] = @week";
            return (await dbConnection.QueryAsync<double>(query, new { playerId, season, week })).FirstOrDefault();
        }

        public async Task<List<T>> GetSeasonDataByTeamIdAndPosition<T>(int teamId, Position position, int season)
        {
            var query = $@"SELECT * FROM [dbo].[{GetSeasonTable(position)}] s WHERE s.[Season] = @season
                            AND EXISTS (SELECT 1 FROM [dbo].PlayerTeam pt WHERE s.PlayerId = pt.PlayerId
                                        AND pt.TeamId = @teamId
                                        AND pt.Season = s.Season)";
            return (await dbConnection.QueryAsync<T>(query, new { teamId, season })).ToList();

        }
        private static string GetWeeklyTable(Position position) => string.Format("Weekly{0}Data", position.ToString());
        private static string GetSeasonTable(Position position) => string.Format("Season{0}Data", position.ToString());
        private static string GetSeasonQueryWhere(bool isPlayer) => isPlayer ? "[PlayerId] = @queryParam" : "[Season] = @queryParam";

    }
}

