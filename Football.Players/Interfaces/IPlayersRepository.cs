﻿using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersRepository
    {
        public Task<int> GetPlayerId(string name);
        public Task<int> CreatePlayer(string Name, int Active, string Position);
        public Task<List<Player>> GetPlayersByPosition(string position, bool activeOnly);
        public Task<List<Player>> GetAllPlayers(int active = 0, string position = "");
        public Task<Player> GetPlayer(int playerId);
        public Task<Player?> GetPlayerByName(string name);
        public Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position);
        public Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position);
        public Task<List<InjuryConcerns>> GetPlayerInjuries(int season);
        public Task<List<Suspensions>> GetPlayerSuspensions(int season);
        public Task<Dictionary<int, double>> GetSeasonProjections(IEnumerable<int> playerIds, int season);
        public Task<double> GetWeeklyProjection(int season, int week, int playerId);
        public Task<int> GetCurrentWeek(int season);
        public Task<List<int>> GetIgnoreList();
        public Task<List<InSeasonInjury>> GetActiveInSeasonInjuries(int season);
        public Task<Dictionary<int, double>> GetGamesPlayedInjuredBySeason(int season);
        public Task<int> PostInSeasonInjury(InSeasonInjury injury);
        public Task<bool> UpdateInjury(InSeasonInjury injury);
        public Task<List<InSeasonTeamChange>> GetInSeasonTeamChanges(int season);
        public Task<bool> UpdateCurrentTeam(int playerId, string newTeam, int season);
        public Task<int> PostTeamChange(InSeasonTeamChange teamChange);
        public Task<int> InactivatePlayers(List<int> playerIds);
        public Task<int> ActivatePlayer(int playerId);
        public Task<List<int>> GetSeasons();
        public Task<bool> CreateRookie(Rookie rookie);
        public Task<List<Rookie>> GetAllRookies();
        public Task<int> UploadSleeperPlayerMap(List<SleeperPlayerMap> playerMap);
        public Task<SleeperPlayerMap?> GetSleeperPlayerMap(int sleeperId);
        public Task<List<SleeperPlayerMap>> GetSleeperPlayerMaps();
        public Task<IEnumerable<TeamChange>> GetAllTeamChanges(int currentSeason, int previousSeason);
        public Task<SeasonInfo> GetSeasonInfo(int season);
    }
}
