using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using System.Data;
using Dapper;

namespace Football.Fantasy.Repository
{
    public class FantasyDataRepository : IFantasyDataRepository
    {
        private readonly IDbConnection _dbConnection;
        public FantasyDataRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season)
        {
            var query = $@"SELECT [PlayerId], [Season], [Games], [FantasyPoints]
                            FROM [dbo].SeasonFantasyData
                            WHERE [PlayerId] = @playerId
                            AND [Season] = @season";
            return (await _dbConnection.QueryAsync<SeasonFantasy>(query, new { playerId, season })).First();
        }
        public async Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Season], [Games], [FantasyPoints]
                            FROM [dbo].SeasonFantasyData
                            WHERE [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<SeasonFantasy>(query, new { playerId })).ToList();
        }
        public async Task<int> PostSeasonFantasy(SeasonFantasy data)
        {
            var query = $@"INSERT INTO [dbo].SeasonFantasyData (PlayerId, Season, Games, FantasyPoints)
                            VALUES (@PlayerId, @Season, @Games, @FantasyPoints)";
            return await _dbConnection.ExecuteAsync(query, data);

        }
        public async Task<List<Player>> GetPlayersByPosition(string position)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                            FROM [dbo].TempPlayer
                            WHERE [Position] = @position";
            return (await _dbConnection.QueryAsync<Player>(query, new { position })).ToList();
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                        FROM [dbo].TempPlayer
                        WHERE [PlayerId] = @playerId";
            return (await _dbConnection.QueryAsync<Player>(query, new { playerId })).First();
        }
    }
}
