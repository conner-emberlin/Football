using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersRepository
    {
        public Task<int> GetPlayerId(string name);
        public Task<int> CreatePlayer(Player player);
        public Task<List<Player>> GetPlayersByPosition(string position);
        public Task<Player> GetPlayer(int playerId);
    }
}
