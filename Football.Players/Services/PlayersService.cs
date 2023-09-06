using Football.Players.Interfaces;
using Football.Players.Models;
using Serilog;

namespace Football.Players.Services
{
    public class PlayersService : IPlayersService
    {
        private readonly IPlayersRepository _playersRepository;

        public PlayersService(IPlayersRepository playersRepository, ILogger logger)
        {
            _playersRepository = playersRepository;
        }

        public async Task<int> GetPlayerId(string name) => await _playersRepository.GetPlayerId(name);
        public async Task<int> CreatePlayer(Player player) => await _playersRepository.CreatePlayer(player);
        public async Task<List<Player>> GetPlayersByPosition(string position) => await _playersRepository.GetPlayersByPosition(position);
        public async Task<Player> GetPlayer(int playerId) => await _playersRepository.GetPlayer(playerId);
    }
}
