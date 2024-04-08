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
    public class RegressionModelService(IPlayersService playerService, IAdvancedStatisticsService advancedStatisticsService, IMarketShareService marketShareService,
        IOptionsMonitor<Season> season) : IRegressionModelService
    {
        private readonly Season _season = season.CurrentValue;
        public async Task<QBModelWeek> QBModelWeek(WeeklyDataQB stat, SnapCount snaps)
        {
            var projection = await playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);
            var fiveThirtyEightValue = await advancedStatisticsService.FiveThirtyEightQBValue(stat.PlayerId);
            var passerRating = await advancedStatisticsService.PasserRating(stat.PlayerId);

            return new QBModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ProjectedPoints = projection,
                PassingAttemptsPerGame = stat.Attempts,
                PassingYardsPerGame = stat.Yards,
                PassingTouchdownsPerGame = stat.TD,
                RushingAttemptsPerGame = stat.RushingAttempts,
                RushingYardsPerGame = stat.RushingYards,
                RushingTouchdownsPerGame = stat.RushingTD,
                SacksPerGame = stat.Sacks,
                FiveThirtyEightValue = fiveThirtyEightValue,
                PasserRating = passerRating,
                SnapsPerGame = snaps.Snaps
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
        public async Task<RBModelWeek> RBModelWeek(WeeklyDataRB stat, SnapCount snaps)
        {
            var projection = await playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);
            var targetShare = (await marketShareService.GetTargetShare(stat.PlayerId)).RBTargetShare;

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
                RBTargetShare = targetShare,
                SnapsPerGame = snaps.Snaps
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
        public async Task<WRModelWeek> WRModelWeek(WeeklyDataWR stat, SnapCount snaps)
        {
            var projection = await playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);

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
                TouchdownsPerGame = stat.TD,
                SnapsPerGame = snaps.Snaps
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
        public async Task<TEModelWeek> TEModelWeek(WeeklyDataTE stat, SnapCount snaps)
        {
            var projection = await playerService.GetSeasonProjection(_season.CurrentSeason, stat.PlayerId);

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
                TouchdownsPerGame = stat.TD,
                SnapsPerGame = snaps.Snaps
            };
        }

        public async Task<DSTModelWeek> DSTModelWeek(WeeklyDataDST stat)
        {
            var teamId = await playerService.GetTeamId(stat.PlayerId);
            var ya = await advancedStatisticsService.YardsAllowedPerGame(teamId);
            return new DSTModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                SacksPerGame = stat.Sacks,
                IntsPerGame = stat.Ints,
                FumRecPerGame = stat.FumblesRecovered,
                TotalTDPerGame = stat.SpecialTD + stat.DefensiveTD,
                SaftiesPerGame = stat.Safties,
                YardsAllowedPerGame = ya,
            };
        }
    }
}
