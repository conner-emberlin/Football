using Football.Projections.Interfaces;
using Football.Projections.Models;
using Dapper;
using System.Data;
using Football.Enums;
using Football.Models;
using Microsoft.Extensions.Options;

namespace Football.Projections.Repository
{
    public class ProjectionRepository(IDbConnection dbConnection, IOptionsMonitor<Season> season) : IProjectionRepository
    {
        private readonly Season _season = season.CurrentValue;
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

        public async Task<IEnumerable<SeasonProjection>?> GetSeasonProjection(int playerId)
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
            var season = _season.CurrentSeason;
            var query = $@"SELECT * FROM [dbo].WeeklyProjections
                        WHERE [Position] = @pos
                            AND [Season] = @season
                            AND [Week] = @week";
            return  dbConnection.Query<WeekProjection>(query, new { pos, season, week });
        }
        public IEnumerable<SeasonProjection> GetSeasonProjectionsFromSQL(Position position, int season)
        {
            var pos = position.ToString();
            var query = $@"SELECT * FROM [dbo].SeasonProjections
                        WHERE [Position] = @pos
                            AND [Season] = @season";
            return dbConnection.Query<SeasonProjection>(query, new { pos, season });
        }

        public async Task<bool> DeleteWeeklyProjection(int playerId, int week, int season)
        {
            var query = $@"DELETE FROM [dbo].WeeklyProjections
                            WHERE [PlayerId] = @playerId
                                AND [Season] = @season
                                AND [Week] = @week";
            return await dbConnection.ExecuteAsync(query, new { playerId, week, season }) > 0;
        }

        public async Task<bool> DeleteSeasonProjection(int playerId, int season)
        {
            var query = $@"DELETE FROM [dbo].SeasonProjections
                            WHERE [PlayerId] = @playerId
                                AND [Season] = @season";
                                
            return await dbConnection.ExecuteAsync(query, new { playerId, season }) > 0;
        }

        public async Task<bool> PostSeasonProjectionConfiguration(SeasonProjectionConfiguration config)
        {
            var query = $@"INSERT INTO SeasonProjectionConfiguration (Season, Position, DateCreated, Filter)
                            VALUES (@Season, @Position, @DateCreated, @Filter)";
            return await dbConnection.ExecuteAsync(query, config) > 0;
        }

        public async Task<bool> PostWeeklyProjectionConfiguration(WeeklyProjectionConfiguration config)
        {
            var query = $@"INSERT INTO WeeklyProjectionConfiguration (Season, Week, Position, DateCreated, Filter)
                            VALUES (@Season, @Week, @Position, @DateCreated, @Filter)";
            return await dbConnection.ExecuteAsync(query, config) > 0;
        }

        public async Task<string?> GetCurrentSeasonProjectionFilter(string position, int season)
        {
            var query = $@"SELECT Filter FROM [dbo].SeasonProjectionConfiguration
                            WHERE [Season] = @season AND [Position] = @position
                                ORDER BY [DateCreated] DESC";
            return (await dbConnection.QueryAsync<string>(query, new {season, position})).FirstOrDefault();
        }

        public async Task<string?> GetCurrentWeekProjectionFilter(string position, int week, int season)
        {
            var query = $@"SELECT Filter FROM [dbo].WeeklyProjectionConfiguration
                            WHERE [Season] = @season AND [Week] = @week AND [Position] = @position
                            ORDER BY DateCreated DESC";
            return (await dbConnection.QueryAsync<string>(query, new { season, week, position })).FirstOrDefault();
        }

    }
}
