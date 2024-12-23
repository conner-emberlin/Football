﻿using Football.Fantasy.Models;
using Football.Fantasy.Interfaces;
using Dapper;
using System.Data;
using Football.Enums;

namespace Football.Fantasy.Repository
{
    public class MatchupAnalysisRepository(IDbConnection dbConnection) : IMatchupAnalysisRepository
    {
       public async Task<int> PostMatchupRankings(List<MatchupRanking> rankings)
        {
            var query = $@"INSERT INTO [dbo].MatchupRanking (Season, Week, TeamId, GamesPlayed, Position, PointsAllowed, AvgPointsAllowed)
                        VALUES (@Season, @Week, @TeamId, @GamesPlayed, @Position, @PointsAllowed, @AvgPointsAllowed)";

            return await dbConnection.ExecuteAsync(query, rankings);
        }

        public async Task<List<MatchupRanking>> GetPositionalMatchupRankingsFromSQL(string position, int season, int week)
        {
            var query = $@"SELECT * FROM [dbo].MatchupRanking 
                        WHERE [Position] = @position
                            AND [Season] = @season
                            AND [Week] = @week";
            return (await dbConnection.QueryAsync<MatchupRanking>(query, new { position, season, week })).ToList();
        }
        public async Task<List<MatchupRanking>> GetCurrentMatchupRankings(int season)
        {
            var query = $@"SELECT * FROM [dbo].MatchupRanking 
                        WHERE [Season] = @season
                            AND [Week] = (SELECT MAX(Week) FROM [dbo].MatchupRanking WHERE [Season] = @season)";
            return (await dbConnection.QueryAsync<MatchupRanking>(query, new { season})).ToList();
        }
    }
}
