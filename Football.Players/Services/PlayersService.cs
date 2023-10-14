using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Football.Players.Services
{
    public class PlayersService : IPlayersService
    {
        private readonly IPlayersRepository _playersRepository;
        private readonly IMemoryCache _cache;
        private readonly Season _season;
        public PlayersService(IPlayersRepository playersRepository, IMemoryCache cache, IOptionsMonitor<Season> season)
        {
            _playersRepository = playersRepository;
            _season = season.CurrentValue;
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
        public async Task<List<Player>> GetPlayersByPosition(PositionEnum position) => await _playersRepository.GetPlayersByPosition(position.ToString());        
        public async Task<Player> GetPlayer(int playerId) => await _playersRepository.GetPlayer(playerId);
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position) => await _playersRepository.GetHistoricalRookies(currentSeason, position);
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position) => await _playersRepository.GetCurrentRookies(currentSeason, position);
        public async Task<int> GetPlayerInjuries(int playerId, int season) => await _playersRepository.GetPlayerInjuries(playerId, season);
        public async Task<int> GetPlayerSuspensions(int playerId, int season) => await _playersRepository.GetPlayerSuspensions(playerId, season);
        public async Task<List<QuarterbackChange>> GetQuarterbackChanges(int season) => await _playersRepository.GetQuarterbackChanges(season);
        public async Task<double> GetEPA(int playerId, int season) => await _playersRepository.GetEPA(playerId, season);
        public async Task<double> GetSeasonProjection(int season, int playerId) => await _playersRepository.GetSeasonProjection(season, playerId);
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId) => await _playersRepository.GetPlayerTeam(season, playerId);
        public async Task<List<PlayerTeam>> GetPlayersByTeam(string team) => await _playersRepository.GetPlayersByTeam(team);
        public Task<TeamMap> GetTeam(int teamId) => _playersRepository.GetTeam(teamId);
        public async Task<int> GetTeamId(string teamName) => await _playersRepository.GetTeamId(teamName);
        public async Task<int> GetTeamId(int playerId) => await _playersRepository.GetTeamId(playerId);
        public async Task<int> GetTeamIdFromDescription(string teamDescription) => await _playersRepository.GetTeamIdFromDescription(teamDescription);
        public async Task<int> GetCurrentWeek(int season) => await _playersRepository.GetCurrentWeek(season);
        public async Task<List<Schedule>> GetUpcomingGames(int playerId)
        {
            var team = await GetPlayerTeam(_season.CurrentSeason, playerId);
            if (team != null)
            {
                return await _playersRepository.GetUpcomingGames(await GetTeamId(team.Team), _season.CurrentSeason, await GetCurrentWeek(_season.CurrentSeason));
            }
            else { return new List<Schedule>(); }
        }
        public async Task<List<Schedule>> GetGames(int season, int week) => await _playersRepository.GetGames(season, week);
        public async Task<List<Schedule>> GetTeamGames(int teamId) => await _playersRepository.GetTeamGames(teamId, _season.CurrentSeason);
        public async Task<List<TeamMap>> GetAllTeams() => await _playersRepository.GetAllTeams();
        public async Task<List<int>> GetIgnoreList() => await _playersRepository.GetIgnoreList();
        public async Task<TeamLocation> GetTeamLocation(int teamId) => await _playersRepository.GetTeamLocation(teamId);
        private List<Player> RetrieveFromCache() =>
                     _cache.TryGetValue("AllPlayers", out List<Player> cachedPlayers) ? cachedPlayers
                     : Enumerable.Empty<Player>().ToList();
    }
}
