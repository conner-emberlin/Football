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
    public class RegressionModelService(IOptionsMonitor<Season> season) : IRegressionModelService
    {
        private readonly Season _season = season.CurrentValue;

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

        public QBModelWeek QBModelWeek(WeeklyDataQB stat)
        {
            return new QBModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                PassingAttemptsPerGame = stat.Attempts,
                PassingYardsPerGame = stat.Yards,
                PassingTouchdownsPerGame = stat.TD,
                RushingAttemptsPerGame = stat.RushingAttempts,
                RushingYardsPerGame = stat.RushingYards,
                RushingTouchdownsPerGame = stat.RushingTD,
                SacksPerGame = stat.Sacks,
            };
        }
        public RBModelWeek RBModelWeek(WeeklyDataRB stat)
        {
            return new RBModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                RushingAttemptsPerGame = stat.RushingAtt,
                RushingYardsPerGame = stat.RushingYds,
                RushingYardsPerAttempt = stat.RushingAtt > 0 ? stat.RushingYds / stat.RushingAtt : 0,
                RushingTouchdownsPerGame = stat.RushingTD,
                ReceptionsPerGame = stat.Receptions,
                ReceivingYardsPerGame = stat.Yards,
                ReceivingTouchdownsPerGame = stat.ReceivingTD,
            };
        }

        public WRModelWeek WRModelWeek(WeeklyDataWR stat)
        {
            return new WRModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                TargetsPerGame = stat.Targets,
                ReceptionsPerGame = stat.Receptions,
                YardsPerGame = stat.Yards,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD
            };
        }

        public TEModelWeek TEModelWeek(WeeklyDataTE stat)
        {
            return new TEModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                TargetsPerGame = stat.Targets,
                ReceptionsPerGame = stat.Receptions,
                YardsPerGame = stat.Yards,
                YardsPerReception = stat.Receptions > 0 ? stat.Yards / stat.Receptions : 0,
                TouchdownsPerGame = stat.TD
            };
        }

        public DSTModelWeek DSTModelWeek(WeeklyDataDST stat, double yardsAllowed)
        {
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
                YardsAllowedPerGame = yardsAllowed,
            };
        }

        public KModelWeek KModelWeek(WeeklyDataK stat)
        {
            return new KModelWeek
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                ExtraPointAttsPerGame = stat.ExtraPoints,
                ExtraPointsPerGame = stat.ExtraPointAttempts,
                FieldGoalsPerGame = stat.FieldGoals,
                FieldGoalAttemptsPerGame = stat.FieldGoalAttempts,
                FiftyPlusFieldGoalsPerGame = stat.Fifty,
            };
        }
    }
}
