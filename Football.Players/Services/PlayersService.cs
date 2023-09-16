using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Football.Players.Services
{
    public class PlayersService : IPlayersService
    {
        private readonly IPlayersRepository _playersRepository;
        private readonly IMemoryCache _cache;
        public PlayersService(IPlayersRepository playersRepository, IMemoryCache cache)
        {
            _playersRepository = playersRepository;
            _cache = cache;
        }
        public async Task<List<Player>> GetAllPlayers()
        {
            if (RetrieveFromCache().Any())
            {
                return RetrieveFromCache();
            }
            else
            {
                var players = await _playersRepository.GetAllPlayers();
                _cache.Set("AllPlayers", players);
                return players;
            }
        }
        public async Task<int> GetPlayerId(string name) => await _playersRepository.GetPlayerId(name);
        public async Task<int> CreatePlayer(Player player) => await _playersRepository.CreatePlayer(player);
        public async Task<List<Player>> GetPlayersByPosition(string position) => await _playersRepository.GetPlayersByPosition(position);        
        public async Task<Player> GetPlayer(int playerId) => await _playersRepository.GetPlayer(playerId);
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position) => await _playersRepository.GetHistoricalRookies(currentSeason, position);
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position) => await _playersRepository.GetCurrentRookies(currentSeason, position);
        public async Task<int> GetPlayerInjuries(int playerId, int season) => await _playersRepository.GetPlayerInjuries(playerId, season);
        public async Task<int> GetPlayerSuspensions(int playerId, int season) => await _playersRepository.GetPlayerSuspensions(playerId, season);
        public async Task<List<QuarterbackChange>> GetQuarterbackChanges(int season) => await _playersRepository.GetQuarterbackChanges(season);
        public async Task<double> GetEPA(int playerId, int season) => await _playersRepository.GetEPA(playerId, season);
        public async Task<double> GetSeasonProjection(int season, int playerId) => await _playersRepository.GetSeasonProjection(season, playerId);
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId) => await _playersRepository.GetPlayerTeam(season, playerId);
        private List<Player> RetrieveFromCache() =>
                     _cache.TryGetValue("AllPlayers", out List<Player> cachedPlayers) ? cachedPlayers
                     : Enumerable.Empty<Player>().ToList();
    }
}
