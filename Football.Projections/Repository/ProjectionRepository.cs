using Football.Projections.Interfaces;
using Football.Projections.Models;
using Dapper;
using System.Data;
using Football.Enums;

namespace Football.Projections.Repository
{
    public class ProjectionRepository(IDbConnection dbConnection) : IProjectionRepository
    {
        public async Task<int> PostSeasonProjections(List<SeasonProjection> projections)
        {
            var query = $@"INSERT INTO [dbo].SeasonProjections (PlayerId, Season, Name, Position, ProjectedPoints)
                        VALUES (@PlayerId, @Season, @Name, @Position, @ProjectedPoints)";

            return await dbConnection.ExecuteAsync(query, projections);
        }

        public async Task<int> PostWeeklyProjections(List<WeekProjection> projections)
        {
            var query = $@"INSERT INTO [dbo].WeeklyProjections (PlayerId, Season, Week, Name, Position, ProjectedPoints)
                        VALUES (@PlayerId, @Season, @Week, @Name, @Position, @ProjectedPoints)";
            return await dbConnection.ExecuteAsync(query, projections);
        }

        public async Task<IEnumerable<SeasonProjection?>> GetSeasonProjection(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Season], [Name], [Position], [ProjectedPoints]
                        FROM [dbo].SeasonProjections
                        WHERE [PlayerId] = @playerId";
            return await dbConnection.QueryAsync<SeasonProjection>(query, new { playerId });
        }
        public async Task<IEnumerable<WeekProjection>?> GetWeeklyProjection(int playerId)
        {
            var query = $@"SELECT * FROM [dbo].WeeklyProjections WHERE [PlayerId] = @playerId";
            return await dbConnection.QueryAsync<WeekProjection>(query, new { playerId});
        }
        public IEnumerable<WeekProjection> GetWeeklyProjectionsFromSQL(Position position, int week)
        {
            var pos = position.ToString();
            var query = $@"SELECT * FROM [dbo].WeeklyProjections
                        WHERE [Position] = @pos
                            AND [Week] = @week";
            return  dbConnection.Query<WeekProjection>(query, new { pos, week });
        }
        public IEnumerable<SeasonProjection> GetSeasonProjectionsFromSQL(Position position, int season)
        {
            var pos = position.ToString();
            var query = $@"SELECT * FROM [dbo].SeasonProjections
                        WHERE [Position] = @pos
                            AND [Season] = @season";
            return dbConnection.Query<SeasonProjection>(query, new { pos, season });
        }
    }
}
