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
        private readonly ISettingsService _settingsService;
        private readonly Season _season;
        public PlayersService(IPlayersRepository playersRepository, IMemoryCache cache, IOptionsMonitor<Season> season, ISettingsService settingsService)
        {
            _playersRepository = playersRepository;
            _season = season.CurrentValue;
            _cache = cache;
            _settingsService = settingsService;
        }
        public async Task<int> GetPlayerId(string name) => await _playersRepository.GetPlayerId(name);
        public async Task<int> CreatePlayer(Player player) => await _playersRepository.CreatePlayer(player);
        public async Task<List<Player>> GetPlayersByPosition(Position position) => await _playersRepository.GetPlayersByPosition(position.ToString());        
        public async Task<Player> GetPlayer(int playerId) => await _playersRepository.GetPlayer(playerId);
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position) => await _playersRepository.GetHistoricalRookies(currentSeason, position);
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position) => await _playersRepository.GetCurrentRookies(currentSeason, position);
        public async Task<int> GetPlayerInjuries(int playerId, int season) => await _playersRepository.GetPlayerInjuries(playerId, season);
        public async Task<int> GetPlayerSuspensions(int playerId, int season) => await _playersRepository.GetPlayerSuspensions(playerId, season);
        public async Task<List<QuarterbackChange>> GetQuarterbackChanges(int season) => await _playersRepository.GetQuarterbackChanges(season);
        public async Task<double> GetEPA(int playerId, int season) => await _playersRepository.GetEPA(playerId, season);
        public async Task<double> GetSeasonProjection(int season, int playerId) => await _playersRepository.GetSeasonProjection(season, playerId);
        public async Task<double> GetWeeklyProjection(int season, int week, int playerId) => await _playersRepository.GetWeeklyProjection(season, week, playerId);
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId) => await _playersRepository.GetPlayerTeam(season, playerId);
        public async Task<TeamMap> GetTeam(int teamId) => await _playersRepository.GetTeam(teamId);
        public async Task<int> GetTeamId(string teamName) => await _playersRepository.GetTeamId(teamName);
        public async Task<int> GetTeamId(int playerId) => await _playersRepository.GetTeamId(playerId);
        public async Task<int> GetTeamIdFromDescription(string teamDescription) => await _playersRepository.GetTeamIdFromDescription(teamDescription);
        public async Task<int> GetCurrentWeek(int season) => await _playersRepository.GetCurrentWeek(season);
        public async Task<List<Schedule>> GetGames(int season, int week) => await _playersRepository.GetGames(season, week);
        public async Task<List<Schedule>> GetTeamGames(int teamId) => await _playersRepository.GetTeamGames(teamId, _season.CurrentSeason);
        public async Task<List<TeamMap>> GetAllTeams() => await _playersRepository.GetAllTeams();
        public async Task<List<int>> GetIgnoreList() => await _playersRepository.GetIgnoreList();
        public async Task<TeamLocation> GetTeamLocation(int teamId) => await _playersRepository.GetTeamLocation(teamId);
        public async Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week) => await _playersRepository.GetScheduleDetails(season, week);
        public async Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season) => await _playersRepository.GetActiveInSeasonInjuries(season);
        public async Task<int> PostInSeasonInjury(InSeasonInjury injury) => await _playersRepository.PostInSeasonInjury(injury);
        public async Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges() => await _playersRepository.GetInSeasonTeamChanges(_season.CurrentSeason);

        public async Task<List<Player>> GetAllPlayers()
        {
            if (_settingsService.GetFromCache<Player>(Cache.AllPlayers, out var cachedValues))
            {
                return cachedValues;
            }
            else
            {
                var players = await _playersRepository.GetAllPlayers();
                _cache.Set(Cache.AllPlayers.ToString(), players);
                return players;
            }
        }
        public async Task<List<PlayerTeam>> GetPlayersByTeam(string team) 
        { 
            var playerTeams = await _playersRepository.GetPlayersByTeam(team, _season.CurrentSeason);
            var teamChanges = await GetInSeasonTeamChanges();
            var newPlayers = teamChanges.Where(t => t.PreviousTeam == team);
            if (newPlayers.Any())
            {
                foreach (var newPlayer in newPlayers)
                {
                    var player = await GetPlayer(newPlayer.PlayerId);
                    playerTeams.Add(new PlayerTeam
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Season = _season.CurrentSeason,
                        Team = team
                    });
                }
            }
            return playerTeams;
        } 
        public async Task<List<Schedule>> GetUpcomingGames(int playerId)
        {
            var team = await GetPlayerTeam(_season.CurrentSeason, playerId);
            if (team != null)
            {
                var teamId = await GetTeamId(team.Team);
                var currentWeek = await GetCurrentWeek(_season.CurrentSeason);
                return await _playersRepository.GetUpcomingGames(teamId, _season.CurrentSeason, currentWeek);
            }
            else return new List<Schedule>();
        }

        public async Task<int> PostInSeasonTeamChange(InSeasonTeamChange teamChange)
        {
            var recordUpdated = await _playersRepository.UpdateCurrentTeam(teamChange.PlayerId, teamChange.NewTeam, _season.CurrentSeason);
            if (recordUpdated)
            {
                return await _playersRepository.PostTeamChange(teamChange);
            }
            else return 0;
        }
    }
}
