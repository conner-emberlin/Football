using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using AutoMapper;
using Serilog;
using Football.Players.Repository;

namespace Football.Players.Services
{
    public class PlayersService(IPlayersRepository playersRepository, IMemoryCache cache, IOptionsMonitor<Season> season,
        ISettingsService settingsService, IMapper mapper, ILogger logger, ISleeperLeagueService sleeperLeagueService) : IPlayersService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<int> GetPlayerId(string name) => await playersRepository.GetPlayerId(name);
        public async Task<Player> CreatePlayer(string name, int active, string position)
        {
            var playerId = await playersRepository.CreatePlayer(name, active, position);
            return new Player { PlayerId = playerId, Name = name, Active = active, Position = position };
        }
        public async Task<int> ActivatePlayer(int playerId) => await playersRepository.ActivatePlayer(playerId);
        public async Task<List<Player>> GetPlayersByPosition(Position position, bool activeOnly = false) => await playersRepository.GetPlayersByPosition(position.ToString(), activeOnly);
        public async Task<Player> GetPlayer(int playerId) => await playersRepository.GetPlayer(playerId);
        public async Task<Player?> GetPlayerByName(string name) => await playersRepository.GetPlayerByName(name);

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
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position) => await playersRepository.GetHistoricalRookies(currentSeason, position);
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position) => await playersRepository.GetCurrentRookies(currentSeason, position);
        public async Task<List<InjuryConcerns>> GetPlayerInjuries(int season) => await playersRepository.GetPlayerInjuries(season);
        public async Task<List<Suspensions>> GetPlayerSuspensions(int season) => await playersRepository.GetPlayerSuspensions(season);
        public async Task<Dictionary<int, double>> GetSeasonProjections(IEnumerable<int> playerIds, int season) => await playersRepository.GetSeasonProjections(playerIds, season);
        public async Task<double> GetWeeklyProjection(int season, int week, int playerId) => await playersRepository.GetWeeklyProjection(season, week, playerId);
        public async Task<IEnumerable<TeamChange>> GetAllTeamChanges(int currentSeason) => await playersRepository.GetAllTeamChanges(currentSeason, currentSeason - 1);
        public async Task<TeamMap> GetTeam(int teamId) => await playersRepository.GetTeam(teamId);
        public async Task<int> GetTeamId(string teamName) => await playersRepository.GetTeamId(teamName);
        public async Task<int> GetTeamId(int playerId) => await playersRepository.GetTeamId(playerId);
        public async Task<int> GetTeamIdFromDescription(string teamDescription) => await playersRepository.GetTeamIdFromDescription(teamDescription);
        public async Task<int> GetCurrentWeek(int season) => await playersRepository.GetCurrentWeek(season);
        public async Task<List<Schedule>> GetGames(int season, int week) => await playersRepository.GetGames(season, week);
        public async Task<List<Schedule>> GetTeamGames(int teamId) => await playersRepository.GetTeamGames(teamId, _season.CurrentSeason);
        public async Task<List<TeamMap>> GetAllTeams() => await playersRepository.GetAllTeams();
        public async Task<List<int>> GetIgnoreList() => await playersRepository.GetIgnoreList();
        public async Task<TeamLocation> GetTeamLocation(int teamId) => await playersRepository.GetTeamLocation(teamId);
        public async Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week) => await playersRepository.GetScheduleDetails(season, week);
        public async Task<IEnumerable<Schedule>> GetByeWeeks(int season) => await playersRepository.GetByeWeeks(season);
        public async Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season) => await playersRepository.GetActiveInSeasonInjuries(season);
        public async Task<Dictionary<int, double>> GetGamesPlayedInjuredBySeason(int season) => await playersRepository.GetGamesPlayedInjuredBySeason(season);
        public async Task<int> PostInSeasonInjury(InSeasonInjury injury) => await playersRepository.PostInSeasonInjury(injury);
        public async Task<bool> UpdateInjury(InSeasonInjury injury) => await playersRepository.UpdateInjury(injury);
        public async Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges() => await playersRepository.GetInSeasonTeamChanges(_season.CurrentSeason);
        public async Task<List<int>> GetSeasons() => await playersRepository.GetSeasons();
        public async Task<bool> CreateRookie(Rookie rookie) => await playersRepository.CreateRookie(rookie);
        public async Task<List<Rookie>> GetAllRookies() => await playersRepository.GetAllRookies();
        public async Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId) => await playersRepository.GetSleeperPlayerMap(sleeperId);
        public async Task<int> InactivatePlayers(List<int> playerIds)
        {
            var updated = await playersRepository.InactivatePlayers(playerIds);
            if (updated > 0) cache.Remove(Cache.AllPlayers.ToString());
            return updated;
        }
   
        public async Task<PlayerTeam?> GetPlayerTeam(int season, int playerId)
        {
            var player = await GetPlayer(playerId);
            if (player.Position == Position.DST.ToString())
            {
                var teamId = await GetTeamId(playerId);
                var team = await GetTeam(teamId);
                return new PlayerTeam
                {
                    PlayerId = playerId,
                    Name = team.TeamDescription,
                    Season = season,
                    Team = team.Team,
                    TeamId = teamId
                };
            }
            else return await playersRepository.GetPlayerTeam(season, playerId);
        }
        public async Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds) => await playersRepository.GetPlayerTeams(season, playerIds);
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
        public async Task<List<Player>> GetAllPlayers()
        {
            if (settingsService.GetFromCache<Player>(Cache.AllPlayers, out var cachedValues))
                return cachedValues;
            else
            {
                var players = await playersRepository.GetAllPlayers();
                cache.Set(Cache.AllPlayers.ToString(), players);
                return players;
            }
        }

        public async Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, Position position, int season, bool activeOnly = false) => await playersRepository.GetPlayersByTeamIdAndPosition(teamId, position.ToString(), season, activeOnly);
        public async Task<List<PlayerTeam>> GetPlayersByTeam(string team)
        {
            var playerTeams = await playersRepository.GetPlayersByTeam(team, _season.CurrentSeason);
            var teamId = await GetTeamId(team);
            var teamMap = await GetTeam(teamId);
            playerTeams.Add(new PlayerTeam
            {
                PlayerId = teamMap.PlayerId,
                Name = teamMap.TeamDescription,
                Season = _season.CurrentSeason,
                Team = team,
                TeamId = teamId
            });
            var formerPlayers = (await GetInSeasonTeamChanges()).Where(t => t.PreviousTeam == team);
            if (formerPlayers.Any())
            {
                foreach (var formerPlayer in formerPlayers)
                {
                    var player = await GetPlayer(formerPlayer.PlayerId);
                    playerTeams.Add(new PlayerTeam
                    {
                        PlayerId = player.PlayerId,
                        Name = player.Name,
                        Season = _season.CurrentSeason,
                        Team = team,
                        TeamId = teamMap.TeamId
                    });
                }
            }
            return playerTeams;
        }
        public async Task<List<Schedule>> GetUpcomingGames(int playerId)
        {
            var team = await GetPlayerTeam(_season.CurrentSeason, playerId);
            return team != null ? await playersRepository.GetUpcomingGames(await GetTeamId(team.Team), _season.CurrentSeason, await GetCurrentWeek(_season.CurrentSeason)) : [];
        }

        public async Task<int> PostInSeasonTeamChange(InSeasonTeamChange teamChange)
        {
            return await playersRepository.UpdateCurrentTeam(teamChange.PlayerId, teamChange.NewTeam, _season.CurrentSeason) ?
                   await playersRepository.PostTeamChange(teamChange) : 0;
        }

        

        public async Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week) => await playersRepository.GetWeeklySchedule(season, week);

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
                                    PlayerTeam = await GetPlayerTeam(_season.CurrentSeason, sleeperMap.PlayerId),
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
        public async Task<int> GetGamesBySeason(int season) => (await playersRepository.GetSeasonInfo(season)).Games;
        public async Task<int> GetWeeksBySeason(int season) => (await playersRepository.GetSeasonInfo(season)).Weeks;
        public async Task<int> GetCurrentSeasonGames() => await GetGamesBySeason(_season.CurrentSeason);
        public async Task<int> GetCurrentSeasonWeeks() => await GetWeeksBySeason(_season.CurrentSeason);
        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsInDivision(int teamId) =>  await playersRepository.GetTeamsLeagueInformationByDivision((await GetTeamLeagueInformation(teamId)).Division, teamId);
        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsByDivision(Division division) => await playersRepository.GetTeamsLeagueInformationByDivision(division.ToString());
        public async Task<IEnumerable<TeamLeagueInformation>> GetTeamsByConference(Conference conference) => await playersRepository.GetTeamLeagueInformationByConference(conference.ToString());
        public async Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId) => await playersRepository.GetTeamLeagueInformation(teamId);
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
