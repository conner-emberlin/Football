using Football.Fantasy.Analysis.Models;
using Football.Fantasy.Analysis.Interfaces;
using Dapper;
using System.Data;

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
    }
}
