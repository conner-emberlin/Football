using Football.Models;
using Football.Data.Models;
using Football.Players.Interfaces;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using Football.Fantasy.Analysis.Interfaces;
using Microsoft.Extensions.Options;
using Football.Statistics.Interfaces;

namespace Football.Projections.Services
{
    public class RegressionModelService : IRegressionModelService
    {
        private readonly IPlayersService _playerService;
        private readonly IAdvancedStatisticsService _advandedStatisticsService;
        private readonly IMarketShareService _marketShareService;
        private readonly Season _season;

        public RegressionModelService(IPlayersService playerService, IAdvancedStatisticsService advancedStatisticsService, IMarketShareService marketShareService,
            IOptionsMonitor<Season> season)
        {
            _playerService = playerService;
            _advandedStatisticsService = advancedStatisticsService;
            _marketShareService = marketShareService;
            _season = season.CurrentValue;
        }
        public QBModelSeason QBModelSeason(SeasonDataQB stat)
        {
            return new QBModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                PassingAttemptsPerGame = stat.Attempts / stat.Games,
                PassingYardsPerGame = stat.Yards / stat.Games,
                PassingTouchdownsPerGame = stat.TD / stat.Games,
                RushingAttemptsPerGame = stat.RushingAttempts / stat.Games,
                RushingYardsPerGame = stat.RushingYards / stat.Games,
                RushingTouchdownsPerGame = stat.RushingTD / stat.Games,
                SacksPerGame = stat.Sacks / stat.Games
            };
        }
        public async Task<QBModelWeek> QBModelWeek(WeeklyDataQB stat)
        {
            var projection = await _playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);
            var fiveThirtyEightValue = await _advandedStatisticsService.FiveThirtyEightQBValue(stat.PlayerId);
            var passerRating = await _advandedStatisticsService.PasserRating(stat.PlayerId);

            return new QBModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection,
                PassingAttemptsPerGame = stat.Attempts,
                PassingYardsPerGame = stat.Yards,
                PassingTouchdownsPerGame = stat.TD,
                RushingAttemptsPerGame= stat.RushingAttempts,
                RushingYardsPerGame = stat.RushingYards,
                RushingTouchdownsPerGame = stat.RushingTD,
                SacksPerGame = stat.Sacks,
                FiveThirtyEightValue = fiveThirtyEightValue,
                PasserRating = passerRating
            };
        }
        public RBModelSeason RBModelSeason(SeasonDataRB stat)
        {
            return new RBModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                RushingAttemptsPerGame = stat.RushingAtt / stat.Games,
                RushingYardsPerGame = stat.RushingYds / stat.Games,
                RushingYardsPerAttempt = stat.RushingAtt > 0 ? stat.RushingYds / stat.RushingAtt : 0,
                RushingTouchdownsPerGame = stat.RushingTD / stat.Games,
                ReceptionsPerGame = stat.Receptions / stat.Games,
                ReceivingYardsPerGame = stat.Yards / stat.Games,
                ReceivingTouchdownsPerGame = stat.ReceivingTD / stat.Games
            };
        }
        public async Task<RBModelWeek> RBModelWeek(WeeklyDataRB stat)
        {
            var projection = await _playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);
            var targetShare = (await _marketShareService.GetTargetShare(stat.PlayerId)).RBTargetShare;
            return new RBModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection,
                RushingAttemptsPerGame = stat.RushingAtt,
                RushingYardsPerGame = stat.RushingYds,
                RushingYardsPerAttempt = stat.RushingAtt > 0 ? stat.RushingYds / stat.RushingAtt : 0,
                RushingTouchdownsPerGame = stat.RushingTD,
                ReceptionsPerGame = stat.Receptions,
                ReceivingYardsPerGame = stat.Yards,
                ReceivingTouchdownsPerGame = stat.ReceivingTD,
                RBTargetShare = targetShare
            };
        }
        public WRModelSeason WRModelSeason(SeasonDataWR stat)
        {
            return new WRModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                TargetsPerGame = stat.Targets / stat.Games,
                ReceptionsPerGame = stat.Receptions / stat.Games,
                YardsPerGame = stat.Yards / stat.Games,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD / stat.Games
            };
        }
        public async Task<WRModelWeek> WRModelWeek(WeeklyDataWR stat)
        {
            var projection = await _playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);
            return new WRModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection,
                TargetsPerGame = stat.Targets,
                ReceptionsPerGame = stat.Receptions,
                YardsPerGame = stat.Yards,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards/stat.Receptions : 0,
                TouchdownsPerGame = stat.TD
            };
        }
        public TEModelSeason TEModelSeason(SeasonDataTE stat)
        {
            return new TEModelSeason
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                TargetsPerGame = stat.Targets / stat.Games,
                ReceptionsPerGame = stat.Receptions / stat.Games,
                YardsPerGame = stat.Yards / stat.Games,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD / stat.Games
            };
        }
        public async Task<TEModelWeek> TEModelWeek(WeeklyDataTE stat)
        {
            var projection = await _playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);
            return new TEModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection,
                TargetsPerGame = stat.Targets,
                ReceptionsPerGame = stat.Receptions,
                YardsPerGame = stat.Yards,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD
            };
        }
    }
}
