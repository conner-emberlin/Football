﻿using Football.Data.Models;
using Football.Enums;
using Football.Statistics.Interfaces;

namespace Football.Statistics.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }
        public async Task<List<T>> GetSeasonData<T>(PositionEnum position, int queryParam, bool isPlayer) => await _statisticsRepository.GetSeasonData<T>(position, queryParam, isPlayer);
        public async Task<List<T>> GetWeeklyData<T>(PositionEnum position, int playerId) => await _statisticsRepository.GetWeeklyData<T>(position, playerId);
        public async Task<List<T>> GetWeeklyData<T>(PositionEnum position, int season, int week) => await _statisticsRepository.GetWeeklyData<T>(position, season, week);
        public async Task<List<GameResult>> GetGameResults(int season, int week) => await _statisticsRepository.GetGameResults(season, week);
        public async Task<List<WeeklyRosterPercent>> GetWeeklyRosterPercentages(int season, int week) => await _statisticsRepository.GetWeeklyRosterPercentages(season, week);

    }
}