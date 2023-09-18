using Football.Players.Models;
using System.Data;
using Dapper;
using Football.Players.Interfaces;

namespace Football.Players.Repository
{
    
    public class PlayersRepository : IPlayersRepository
    {
        private readonly IDbConnection _dbConnection;
        public PlayersRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> GetPlayerId(string name)
        {
            var query = $@"SELECT [PlayerId] FROM [dbo].TempPlayer
                            WHERE [Name] = @name
                            ORDER BY [Active] DESC";
            return (await _dbConnection.QueryAsync<int>(query, new { name })).FirstOrDefault();
        }
        public async Task<int> CreatePlayer(Player player)
        {
            var query = $@"INSERT INTO [dbo].TempPlayer (Name, Position, Active)
                        VALUES (@Name, @Position, @Active)";
            return await _dbConnection.ExecuteAsync(query, player);
        }
        public async Task<List<Player>> GetAllPlayers()
        {
            var query = $@"SELECT [PlayerId], [Name], [Position], [Active]
                        FROM [dbo].TempPlayer   
                        ";
            return (await _dbConnection.QueryAsync<Player>(query)).ToList();
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
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position)
        {
            var query = $@"SELECT [PlayerId], [TeamDrafted], [Position], [RookieSeason], [DraftPosition], [DeclareAge]
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] < @currentSeason
                        AND [Position] = @position";
            return (await _dbConnection.QueryAsync<Rookie>(query, new {currentSeason, position})).ToList();
        }
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position)
        {
            var query = $@"SELECT [PlayerId], [TeamDrafted], [Position], [RookieSeason], [DraftPosition], [DeclareAge]
                        FROM [dbo].Rookie
                        WHERE [RookieSeason] = @currentSeason
                        AND [Position] = @position";
            return (await _dbConnection.QueryAsync<Rookie>(query, new { currentSeason, position })).ToList();
        }
        public async Task<int> GetPlayerInjuries(int playerId, int season)
        {
            var query = $@"SELECT [Games] FROM [dbo].InjuryConcerns
                        WHERE [PlayerId] = @playerId
                        AND [Season] = @season";
            return (await _dbConnection.QueryAsync<int>(query, new { playerId, season })).FirstOrDefault();
        }
        public async Task<int> GetPlayerSuspensions(int playerId, int season)
        {
            var query = $@"SELECT [Length] FROM [dbo].Suspensions
                            WHERE [PlayerId] = @playerId
                            AND [Season] = @season";
            return (await _dbConnection.QueryAsync<int>(query, new { playerId, season })).FirstOrDefault();
        }
        public async Task<List<QuarterbackChange>> GetQuarterbackChanges(int season)
        {
            var query = $@"SELECT [PlayerId], [Season], [PreviousQB], [CurrentQB]
                        FROM [dbo].QuarterbackChanges
                        WHERE [Season] = @season";
            return (await _dbConnection.QueryAsync<QuarterbackChange>(query, new {season})).ToList();
        }
        public async Task<double> GetEPA(int playerId, int season)
        {
            var query = $@"SELECT [EPA] FROM [dbo].[EPA] WHERE [PlayerId] = @playerId AND [Season] = @season";
            return (await _dbConnection.QueryAsync<double>(query, new {playerId, season})).FirstOrDefault();
        }
        public async Task<double> GetSeasonProjection(int season, int playerId)
        {
            var query = $@"SELECT [ProjectedPoints]
                        FROM [dbo].SeasonProjections
                        WHERE [PlayerId] = @playerId
                            AND [Season] = @season";
            return (await _dbConnection.QueryAsync<double>(query, new { season, playerId })).FirstOrDefault(); 
        }
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId)
        {
            var query = $@"SELECT [PlayerId], [Name], [Season], [Team]
                        FROM [dbo].PlayerTeam
                        WHERE [Season] = @season
                            AND [PlayerId] = @playerId";

            return (await _dbConnection.QueryAsync<PlayerTeam>(query, new { season, playerId })).FirstOrDefault();
        }
        public async Task<int> GetTeamId(string teamName)
        {
            var query = $@"SELECT [TeamId] FROM [dbo].TeamMap
                        WHERE [Team] = @teamName";
            return (await _dbConnection.QueryAsync<int>(query, new { teamName })).FirstOrDefault();
        }
    }
}
