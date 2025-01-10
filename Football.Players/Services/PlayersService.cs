using AutoMapper;
using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace Football.Players.Services
{
    public class PlayersService(IPlayersRepository playersRepository, IMemoryCache cache, IOptionsMonitor<Season> season,
        ISettingsService settingsService, IMapper mapper, ILogger logger, ISleeperLeagueService sleeperLeagueService) : IPlayersService
    {
        private readonly Season _season = season.CurrentValue;

        public Task<int> GetPlayerId(string name) => playersRepository.GetPlayerId(name);
        public async Task<Player> CreatePlayer(string name, int active, string position)
        {
            var playerId = await playersRepository.CreatePlayer(name, active, position);
            return new Player { PlayerId = playerId, Name = name, Active = active, Position = position };
        }
        public Task<int> ActivatePlayer(int playerId) => playersRepository.ActivatePlayer(playerId);
        public Task<List<Player>> GetPlayersByPosition(Position position, bool activeOnly = false) => playersRepository.GetPlayersByPosition(position.ToString(), activeOnly);
        public Task<Player> GetPlayer(int playerId) => playersRepository.GetPlayer(playerId);
        public Task<Player?> GetPlayerByName(string name) => playersRepository.GetPlayerByName(name);
        public async Task<Player> RetrievePlayer(string name, Position position, bool activatePlayer = false)
        {
            var player = await GetPlayerByName(name);
            if (player == null)
            {
                var active = activatePlayer ? 1 : 0;
                player = await CreatePlayer(name, active, position.ToString());
                logger.Information("New Player Added. Name: {0}, PlayerId: {1}", player.Name, player.PlayerId);
            }
            else if(player.Active == 0 && activatePlayer)
            {
                var updated = await ActivatePlayer(player.PlayerId);
                if (updated > 0)
                {
                    logger.Information("Player activated. Name: {0}, PlayerId {1}", player.Name, player.PlayerId);
                    player.Active = 1;
                }
            }
            return player;
        }
        public Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position) => playersRepository.GetHistoricalRookies(currentSeason, position);
        public Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position) => playersRepository.GetCurrentRookies(currentSeason, position);
        public Task<List<InjuryConcerns>> GetPlayerInjuries(int season) => playersRepository.GetPlayerInjuries(season);
        public Task<List<Suspensions>> GetPlayerSuspensions(int season) => playersRepository.GetPlayerSuspensions(season);
        public Task<Dictionary<int, double>> GetSeasonProjections(IEnumerable<int> playerIds, int season) => playersRepository.GetSeasonProjections(playerIds, season);
        public Task<double> GetWeeklyProjection(int season, int week, int playerId) => playersRepository.GetWeeklyProjection(season, week, playerId);
        public Task<IEnumerable<TeamChange>> GetAllTeamChanges(int currentSeason) => playersRepository.GetAllTeamChanges(currentSeason, currentSeason - 1);
        public Task<int> GetCurrentWeek(int season) => playersRepository.GetCurrentWeek(season);
        public Task<List<int>> GetIgnoreList() => playersRepository.GetIgnoreList();
        public Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season) => playersRepository.GetActiveInSeasonInjuries(season);
        public Task<Dictionary<int, double>> GetGamesPlayedInjuredBySeason(int season) => playersRepository.GetGamesPlayedInjuredBySeason(season);
        public Task<List<PlayerInjury>> GetPlayerInjuryHistory(int playerId) => playersRepository.GetPlayerInjuryHistory(playerId);
        public Task<int> PostInSeasonInjury(InSeasonInjury injury) => playersRepository.PostInSeasonInjury(injury);
        public Task<bool> UpdateInjury(InSeasonInjury injury) => playersRepository.UpdateInjury(injury);
        public Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges() => playersRepository.GetInSeasonTeamChanges(_season.CurrentSeason);
        public Task<List<int>> GetSeasons() => playersRepository.GetSeasons();
        public Task<bool> CreateRookie(Rookie rookie) => playersRepository.CreateRookie(rookie);
        public Task<List<Rookie>> GetAllRookies() => playersRepository.GetAllRookies();
        public Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId) => playersRepository.GetSleeperPlayerMap(sleeperId);
        public Task<int> GetCurrentSeasonGames() => GetGamesBySeason(_season.CurrentSeason);
        public Task<int> GetCurrentSeasonWeeks() => GetWeeksBySeason(_season.CurrentSeason);
        public async Task<int> GetGamesBySeason(int season) => (await playersRepository.GetSeasonInfo(season)).Games;
        public async Task<int> GetWeeksBySeason(int season) => (await playersRepository.GetSeasonInfo(season)).Weeks;
        public async Task<int> InactivatePlayers(List<int> playerIds)
        {
            var updated = await playersRepository.InactivatePlayers(playerIds);
            if (updated > 0) cache.Remove(Cache.AllPlayers.ToString());
            return updated;
        }   
        public async Task<List<PlayerInjury>> GetPlayerInjuries()
        {
            List<PlayerInjury> playerInjuries = [];
            var injuries = await GetActiveInSeasonInjuries(_season.CurrentSeason);
            foreach (var injury in injuries)
            {
                var playerInjury = mapper.Map<PlayerInjury>(injury);
                playerInjury.Player = await GetPlayer(injury.PlayerId);
                playerInjuries.Add(playerInjury);
            }
            return playerInjuries;
        }
        public async Task<List<Player>> GetAllPlayers(int active = 0, string position = "")
        {
            if (cache.TryGetValue<List<Player>>(Cache.AllPlayers + active.ToString() + position, out var cachedPlayers) && cachedPlayers != null) return cachedPlayers;
            var players = await playersRepository.GetAllPlayers(active, position);
            cache.Set(Cache.AllPlayers.ToString() + active.ToString() + position, players);
            return players;
        }
        public async Task<int> PostInSeasonTeamChange(InSeasonTeamChange teamChange)
        {
            return await playersRepository.UpdateCurrentTeam(teamChange.PlayerId, teamChange.NewTeam, _season.CurrentSeason) ?
                   await playersRepository.PostTeamChange(teamChange) : 0;
        }        
        public async Task<List<TrendingPlayer>> GetTrendingPlayers()
        {
            if (settingsService.GetFromCache<TrendingPlayer>(Cache.TrendingPlayers, out var cachedTrending))
                return cachedTrending;
            else
            {
                var sleeperTrendingPlayers = await sleeperLeagueService.GetSleeperTrendingPlayers();
                List<TrendingPlayer> trendingPlayers = [];

                if (sleeperTrendingPlayers != null)
                {
                    foreach (var sleeperPlayer in sleeperTrendingPlayers)
                    {
                        if (int.TryParse(sleeperPlayer.SleeperPlayerId, out var sleeperId))
                        {
                            var sleeperMap = await GetSleeperPlayerMap(sleeperId);
                            if (sleeperMap != null)
                            {
                                trendingPlayers.Add(new TrendingPlayer
                                {
                                    Player = await GetPlayer(sleeperMap.PlayerId),
                                    Adds = sleeperPlayer.Adds
                                });
                            }

                        }
                    }
                }
                cache.Set(Cache.TrendingPlayers.ToString(), trendingPlayers);
                return trendingPlayers;
            }
        }

        public async Task<int> UploadSleeperPlayerMap()
        {
            var sleeperPlayers = await sleeperLeagueService.GetSleeperPlayers();
            var existingMaps = await playersRepository.GetSleeperPlayerMaps();
            var playerMap = (await GetSleeperPlayerMap(sleeperPlayers)).Where(s => !existingMaps.Any(e => e.PlayerId == s.PlayerId)).ToList();
            return await playersRepository.UploadSleeperPlayerMap(playerMap);
        }
        private async Task<List<SleeperPlayerMap>> GetSleeperPlayerMap(List<SleeperPlayer> sleeperPlayers)
        {
            List<SleeperPlayerMap> playerMap = [];
            if (sleeperPlayers.Count > 0)
            {
                foreach (var sp in sleeperPlayers)
                {
                    if (!string.IsNullOrWhiteSpace(sp.PlayerName))
                    {
                        var playerId = await GetPlayerId(sp.PlayerName);
                        if (playerId > 0)
                        {
                            playerMap.Add(new SleeperPlayerMap
                            {
                                SleeperPlayerId = sp.SleeperPlayerId,
                                PlayerId = playerId
                            });
                        }
                    }
                }
            }
            return playerMap;
        }
        
    }
}
