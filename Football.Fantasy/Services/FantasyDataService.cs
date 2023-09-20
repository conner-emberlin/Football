﻿using Football.Data.Models;
using Football.Fantasy.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Serilog;
using System.Runtime.CompilerServices;

namespace Football.Fantasy.Services
{
    public class FantasyDataService : IFantasyDataService
    {
        private readonly IFantasyDataRepository _fantasyData;
        private readonly IFantasyCalculator _calculator;
        private readonly IStatisticsService _statisticsService;
        private readonly IPlayersService _playersService;
        private readonly ILogger _logger;
        public FantasyDataService(IFantasyDataRepository fantasyData, IFantasyCalculator calculator, 
            IStatisticsService statistics, IPlayersService playersService, ILogger logger)
        {
            _fantasyData = fantasyData;
            _calculator = calculator;
            _statisticsService = statistics;
            _playersService = playersService;
            _logger = logger;
        }
        public async Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season) => await _fantasyData.GetSeasonFantasy(playerId, season);
        public async Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId) => await _fantasyData.GetSeasonFantasy(playerId);
        public async Task<int> PostSeasonFantasy(int season, string position)
        {
            List<SeasonFantasy> seasonFantasy = new();
            switch (position)
            {
                case "QB": 
                    foreach(var data in await _statisticsService.GetSeasonDataQBBySeason(season))
                    {
                        seasonFantasy.Add(_calculator.CalculateQBFantasy(data));
                    }
                    break;
                case "RB":
                    foreach (var data in await _statisticsService.GetSeasonDataRBBySeason(season))
                    {
                        seasonFantasy.Add(_calculator.CalculateRBFantasy(data));
                    }
                    break;
                case "WR":
                    foreach (var data in await _statisticsService.GetSeasonDataWRBySeason(season))
                    {
                        seasonFantasy.Add(_calculator.CalculateWRFantasy(data));
                    }
                    break;
                case "TE":
                    foreach (var data in await _statisticsService.GetSeasonDataTEBySeason(season))
                    {
                        seasonFantasy.Add(_calculator.CalculateTEFantasy(data));
                    }
                    break;
                case "DST":
                    foreach (var data in await _statisticsService.GetSeasonDataDSTBySeason(season))
                    {
                        seasonFantasy.Add(_calculator.CalculateDSTFantasy(data));
                    }
                    break;
                default: _logger.Error("Invalid position {position}", position); break;
            }
            var count = 0;
            foreach(var fp in seasonFantasy)
            {
               count += await  _fantasyData.PostSeasonFantasy(fp);
            }
            return count;
        }

        public async Task<int> PostWeeklyFantasy(int season, int week, string position)
        {
            List<WeeklyFantasy> weeklyFantasy = new();
            switch (position)
            {
                case "QB": 
                    foreach(var data in await _statisticsService.GetWeeklyDataQB(season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateQBFantasy(data));
                    }
                    break;
                case "RB":
                    foreach (var data in await _statisticsService.GetWeeklyDataRB(season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateRBFantasy(data));
                    }
                    break;
                case "WR":
                    foreach (var data in await _statisticsService.GetWeeklyDataWR(season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateWRFantasy(data));
                    }
                    break;
                case "TE":
                    foreach (var data in await _statisticsService.GetWeeklyDataTE(season, week))
                    {
                        weeklyFantasy.Add(_calculator.CalculateTEFantasy(data));
                    }
                    break;
                default: _logger.Error("Invalid position {position}", position); break;
            }
            var count = 0;
            foreach(var fp in weeklyFantasy)
            {
                count += await _fantasyData.PostWeeklyFantasy(fp);
            }
            return count;
        }

        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int playerId) => await _fantasyData.GetWeeklyFantasy(playerId);

        public async Task<List<WeeklyFantasy>> GetWeeklyFantasy(int season, int week) => await _fantasyData.GetWeeklyFantasy(season, week);
        public async Task<List<SeasonFantasy>> GetCurrentFantasyTotals(int season)
        {
            var currentWeek = await _playersService.GetCurrentWeek(season);
            List<WeeklyFantasy> weeklyFantasy = new();
            for (int i = 1; i < currentWeek; i++)
            {
                foreach (var s in await GetWeeklyFantasy(season, i))
                {
                    weeklyFantasy.Add(s);
                }
            }
            return weeklyFantasy.GroupBy(w => w.PlayerId)
                                .Select(gw => new SeasonFantasy
                                {
                                    PlayerId = gw.Key,
                                    Season = season,
                                    Games = currentWeek - 1,
                                    FantasyPoints = Math.Round(gw.Sum(f => f.FantasyPoints),2),
                                    Name = gw.Select(g => g.Name).First(),
                                    Position = gw.Select(g => g.Position).First()
                                }).OrderByDescending(w => w.FantasyPoints).ToList();                                                   
        }
        

    }
}
