using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Football.Players.Services
{
    public class PlayersService(IPlayersRepository playersRepository, IMemoryCache cache, IOptionsMonitor<Season> season, ISettingsService settingsService) : IPlayersService
    {
        private readonly Season _season = season.CurrentValue;

        public async Task<int> GetPlayerId(string name) => await playersRepository.GetPlayerId(name);
        public async Task<int> CreatePlayer(Player player) => await playersRepository.CreatePlayer(player);
        public async Task<List<Player>> GetPlayersByPosition(Position position) => await playersRepository.GetPlayersByPosition(position.ToString());        
        public async Task<Player> GetPlayer(int playerId) => await playersRepository.GetPlayer(playerId);
        public async Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position) => await playersRepository.GetHistoricalRookies(currentSeason, position);
        public async Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position) => await playersRepository.GetCurrentRookies(currentSeason, position);
        public async Task<int> GetPlayerInjuries(int playerId, int season) => await playersRepository.GetPlayerInjuries(playerId, season);
        public async Task<int> GetPlayerSuspensions(int playerId, int season) => await playersRepository.GetPlayerSuspensions(playerId, season);
        public async Task<List<QuarterbackChange>> GetQuarterbackChanges(int season) => await playersRepository.GetQuarterbackChanges(season);
        public async Task<double> GetEPA(int playerId, int season) => await playersRepository.GetEPA(playerId, season);
        public async Task<double> GetSeasonProjection(int season, int playerId) => await playersRepository.GetSeasonProjection(season, playerId);
        public async Task<double> GetWeeklyProjection(int season, int week, int playerId) => await playersRepository.GetWeeklyProjection(season, week, playerId);        
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
        public async Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season) => await playersRepository.GetActiveInSeasonInjuries(season);
        public async Task<int> PostInSeasonInjury(InSeasonInjury injury) => await playersRepository.PostInSeasonInjury(injury);
        public async Task<bool> UpdateInjury(InSeasonInjury injury) => await playersRepository.UpdateInjury(injury);
        public async Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges() => await playersRepository.GetInSeasonTeamChanges(_season.CurrentSeason);
        public async Task<List<int>> GetSeasons() => await playersRepository.GetSeasons();
        public async Task<bool> CreateRookie(Rookie rookie) => await playersRepository.CreateRookie(rookie);
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
                var team = await GetTeam(await GetTeamId(playerId));
                return new PlayerTeam
                {
                    PlayerId = playerId,
                    Name = team.TeamDescription,
                    Season = season,
                    Team = team.Team
                };
            }
            else return await playersRepository.GetPlayerTeam(season, playerId);
        }
        public async Task<List<PlayerInjury>> GetPlayerInjuries()
        {
            List<PlayerInjury> playerInjuries = [];
            var injuries = await GetActiveInSeasonInjuries(_season.CurrentSeason);
            foreach (var injury in injuries)
            {
                var player = await GetPlayer(injury.PlayerId);
                playerInjuries.Add(new PlayerInjury
                {
                    InjuryId = injury.InjuryId,
                    Season = injury.Season,
                    PlayerId = injury.PlayerId,
                    InjuryStartWeek = injury.InjuryStartWeek,
                    InjuryEndWeek = injury.InjuryEndWeek,
                    Description = injury.Description,
                    Player = player
                });
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
        public async Task<List<PlayerTeam>> GetPlayersByTeam(string team) 
        { 
            var playerTeams = await playersRepository.GetPlayersByTeam(team, _season.CurrentSeason);
            var teamMap = await GetTeam(await GetTeamId(team));
            playerTeams.Add(new PlayerTeam
            {
                PlayerId = teamMap.PlayerId,
                Name = teamMap.TeamDescription,
                Season = _season.CurrentSeason,
                Team = team
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
                        Team = team
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
    }
}
