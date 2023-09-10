using Football.Projections.Interfaces;
using Football.Projections.Models;
using Dapper;
using System.Data;

namespace Football.Projections.Repository
{
    public class ProjectionRepository : IProjectionRepository
    {
        private readonly IDbConnection _dbConnection;

        public ProjectionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> PostSeasonProjections(SeasonProjection projection)
        {
            var query = $@"INSERT INTO [dbo].SeasonProjections (PlayerId, Season, Name, Position, ProjectedPoints)
                        VALUES (@PlayerId, @Season, @Name, @Position, @ProjectedPoints)";

            return await _dbConnection.ExecuteAsync(query, projection);
        }

        public async Task<SeasonProjection> GetSeasonProjection(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Season], [Name], [Position], [ProjectedPoints]
                        FROM [dbo].SeasonProjections
                        WHERE [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<SeasonProjection>(query, new { playerId })).FirstOrDefault();
        }
    }
}
