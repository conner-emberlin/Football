﻿using Football.Enums;
using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersService
    {
        public Task<List<Player>> GetAllPlayers(int active = 0, string position = "");
        public Task<int> GetPlayerId(string name);             
        public Task<List<Player>> GetPlayersByPosition(Position position, bool activeOnly = false);
        public Task<Player> GetPlayer(int playerId);
        public Task<Player?> GetPlayerByName(string name);
        public Task<Player> CreatePlayer(string name, int active, string position);
        public Task<Player> RetrievePlayer(string name, Position position, bool activatePlayer = false);
        public Task<int> InactivatePlayers(List<int> playerIds);
        public Task<int> ActivatePlayer(int playerId);
        public Task<List<InjuryConcerns>> GetPlayerInjuries(int season);
        public Task<List<PlayerInjury>> GetPlayerInjuries();
        public Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season);
        public Task<Dictionary<int, double>> GetGamesPlayedInjuredBySeason(int season);
        public Task<int> PostInSeasonInjury(InSeasonInjury injury);
        public Task<bool> UpdateInjury(InSeasonInjury injury);
        public Task<List<Suspensions>> GetPlayerSuspensions(int season);
        public Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges();
        public Task<int> PostInSeasonTeamChange(InSeasonTeamChange teamChange);
        public Task<Dictionary<int, double>> GetSeasonProjections(IEnumerable<int> playerIds, int season);
        public Task<double> GetWeeklyProjection(int season, int week, int playerId);
        public Task<int> GetCurrentWeek(int season);
        public Task<List<int>> GetIgnoreList();
        public Task<List<int>> GetSeasons();
        public Task<int> UploadSleeperPlayerMap();
        public Task<List<TrendingPlayer>> GetTrendingPlayers();
        public Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId);
        public Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position);
        public Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position);
        public Task<bool> CreateRookie(Rookie rookie);
        public Task<List<Rookie>> GetAllRookies();
        public Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
        public Task<IEnumerable<PlayerTeam>> GetPlayerTeams(int season, IEnumerable<int> playerIds);
        public Task<List<PlayerTeam>> GetPlayersByTeam(string team);
        public Task<IEnumerable<PlayerTeam>> GetPlayersByTeamIdAndPosition(int teamId, Position position, int season, bool activeOnly = false);
        public Task<int> GetTeamId(string teamName);
        public Task<int> GetTeamId(int playerId);
        public Task<int> GetTeamIdFromDescription(string teamDescription);
        public Task<List<TeamMap>> GetAllTeams();
        public Task<TeamMap> GetTeam(int teamId);
        public Task<TeamLocation> GetTeamLocation(int teamId);
        public Task<List<Schedule>> GetUpcomingGames(int playerId);
        public Task<List<Schedule>> GetGames(int season, int week);
        public Task<List<Schedule>> GetTeamGames(int teamId);
        public Task<List<ScheduleDetails>> GetScheduleDetails(int season, int week);
        public Task<IEnumerable<Schedule>> GetWeeklySchedule(int season, int week);
        public Task<IEnumerable<Schedule>> GetByeWeeks(int season);
        public Task<IEnumerable<TeamChange>> GetAllTeamChanges(int currentSeason);
        public Task<int> GetCurrentSeasonGames();
        public Task<int> GetCurrentSeasonWeeks();
        public Task<int> GetGamesBySeason(int season);
        public Task<int> GetWeeksBySeason(int season);
        public Task<TeamLeagueInformation> GetTeamLeagueInformation(int teamId);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsInDivision(int teamId);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsByDivision(Division division);
        public Task<IEnumerable<TeamLeagueInformation>> GetTeamsByConference(Conference conference);
    }
}
