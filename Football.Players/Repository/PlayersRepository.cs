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
                            WHERE [Name] LIKE '%' + @name + '%'
                            ORDER BY [Active] DESC";
            return await _dbConnection.ExecuteAsync(query, new { name });
        }
        public async Task<int> CreatePlayer(Player player)
        {
            var query = $@"INSERT INTO [dbo].TempPlayer (PlayerId, Name, Position, Active)
                        VALUES (@PlayerId, @Name, @Position, @Active";
            return await _dbConnection.ExecuteAsync(query, player);
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
