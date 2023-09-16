﻿using Football.Players.Models;

namespace Football.Players.Interfaces
{
    public interface IPlayersRepository
    {
        public Task<int> GetPlayerId(string name);
        public Task<int> CreatePlayer(Player player);
        public Task<List<Player>> GetPlayersByPosition(string position);
        public Task<List<Player>> GetAllPlayers();
        public Task<Player> GetPlayer(int playerId);
        public Task<List<Rookie>> GetHistoricalRookies(int currentSeason, string position);
        public Task<List<Rookie>> GetCurrentRookies(int currentSeason, string position);
        public Task<int> GetPlayerInjuries(int playerId, int season);
        public Task<int> GetPlayerSuspensions(int playerId, int season);
        public Task<List<QuarterbackChange>> GetQuarterbackChanges(int season);
        public Task<double> GetEPA(int playerId, int season);
        public Task<double> GetSeasonProjection(int season, int playerId);
        public Task<PlayerTeam?> GetPlayerTeam(int season, int playerId);
    }
}