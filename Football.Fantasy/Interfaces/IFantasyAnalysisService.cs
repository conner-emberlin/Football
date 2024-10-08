﻿using Football.Enums;
using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyAnalysisService
    {
        public Task<List<BoomBust>> GetBoomBusts(Position position);
        public Task<List<FantasyPerformance>> GetFantasyPerformances(Position position);
        public Task<List<FantasyPerformance>> GetFantasyPerformances(int teamId);
        public Task<FantasyPerformance?> GetFantasyPerformance(Player player);
        public Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId);
        public Task<List<FantasyPercentage>> GetFantasyPercentages(Position position);
        public Task<FantasyPercentage> GetQBSeasonFantasyPercentageByPlayerId(int season, int playerId);
        public Task<FantasyPercentage> GetRBSeasonFantasyPercentageByPlayerId(int season, int playerId);
        public Task<List<FantasySplit>?> GetFantasySplits(Position position, int season);
        public Task<List<WeeklyFantasyTrend>> GetWeeklyFantasyTrendsByPosition(Position position, int season);
        public Task<IEnumerable<WeeklyFantasyTrend>> GetPlayerWeeklyFantasyTrends(int playerId, int season);
        
    }
}
