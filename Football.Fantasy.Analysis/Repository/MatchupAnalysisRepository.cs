using Football.Fantasy.Analysis.Models;
using Football.Fantasy.Analysis.Interfaces;
using Dapper;
using System.Data;
using Football.Enums;

namespace Football.Fantasy.Analysis.Repository
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
    }
}
