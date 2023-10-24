using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
using Football.Enums;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class FantasyCalculator : IFantasyCalculator
    {
        private readonly FantasyScoring _scoring;
        public FantasyCalculator(IOptionsMonitor<FantasyScoring> scoring)
        {
            _scoring = scoring.CurrentValue;
        }           
        public SeasonFantasy CalculateQBFantasy(SeasonDataQB stat)
        {
            var points = _scoring.PointsPerPassingTouchdown * stat.TD + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerPassingYard * stat.Yards + _scoring.PointsPerYard * stat.RushingYards
                        - _scoring.PointsPerFumble * stat.Fumbles - _scoring.PointsPerInterception * stat.Int;
            return new SeasonFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Games = stat.Games,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.QB.ToString()
            };
        }
        public WeeklyFantasy CalculateQBFantasy(WeeklyDataQB stat)
        {
            var points = _scoring.PointsPerPassingTouchdown * stat.TD + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerPassingYard * stat.Yards + _scoring.PointsPerYard * stat.RushingYards
                        - _scoring.PointsPerFumble * stat.Fumbles - _scoring.PointsPerInterception * stat.Int;
            return new WeeklyFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                Games = 1,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.QB.ToString()
            };
        }
        public SeasonFantasy CalculateRBFantasy(SeasonDataRB stat)
        {
            var points = _scoring.PointsPerYard * stat.RushingYds + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerTouchdown * stat.ReceivingTD
                        + _scoring.PointsPerYard * stat.Yards - _scoring.PointsPerFumble * stat.Fumbles;
            return new SeasonFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Games = stat.Games,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.RB.ToString()
            };
        }
        public WeeklyFantasy CalculateRBFantasy(WeeklyDataRB stat)
        {
            var points = _scoring.PointsPerYard * stat.RushingYds + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerTouchdown * stat.ReceivingTD
                        + _scoring.PointsPerYard * stat.Yards - _scoring.PointsPerFumble * stat.Fumbles;
            return new WeeklyFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                Games = 1,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.RB.ToString()
            };
        }
        public SeasonFantasy CalculateWRFantasy(SeasonDataWR stat)
        {
            var points = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return new SeasonFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Games = stat.Games,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.WR.ToString()
            };
        }
        public WeeklyFantasy CalculateWRFantasy(WeeklyDataWR stat)
        {
            var points = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return new WeeklyFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                Games = 1,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.WR.ToString()
            };
        }
        public SeasonFantasy CalculateTEFantasy(SeasonDataTE stat)
        {
            var points = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return new SeasonFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Games = stat.Games,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.TE.ToString()
            };
        }
        public WeeklyFantasy CalculateTEFantasy(WeeklyDataTE stat)
        {
            var points = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return new WeeklyFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                Games = 1,
                FantasyPoints = points,
                Name = stat.Name,
                Position = PositionEnum.TE.ToString()
            };
        }
        public WeeklyFantasy CalculateDSTFantasy(WeeklyDataDST stat, WeeklyDataDST opponentStat, GameResult result, int teamId)
        {
            var pointsAllowed = result.WinnerId == teamId ? result.LoserPoints - _scoring.PointsPerTouchdown * opponentStat.DefensiveTD 
                                : result.WinnerPoints - 6 * opponentStat.DefensiveTD;

            var pointsAllowedFantasy = pointsAllowed == 0 ? _scoring.ZeroPointsAllowed
                                     : 1 <= pointsAllowed && pointsAllowed <= 6 ? _scoring.OneToSixPointsAllowed
                                     : 7 <= pointsAllowed && pointsAllowed <= 13 ? _scoring.SevenToThirteenPointsAllowed
                                     : 14 <= pointsAllowed && pointsAllowed <= 17 ? _scoring.FourteenToSeventeenPointsAllowed
                                     : 18 <= pointsAllowed && pointsAllowed <= 27 ? _scoring.EighteenToTwentySevenPointsAllowed
                                     : 28 <= pointsAllowed && pointsAllowed <= 34 ? _scoring.TwentyEightToThirtyFourPointsAllowed
                                     : 35 <= pointsAllowed && pointsAllowed <= 45 ? _scoring.ThirtyFiveToFourtyFivePointsAllowed
                                     : _scoring.FourtySixOrMorePointsAllowed;

            var fantasyPoints = pointsAllowedFantasy
                                + stat.Sacks * _scoring.PointsPerSack
                                + stat.Ints * _scoring.PointsPerInterception
                                + stat.FumblesRecovered * _scoring.PointsPerFumble
                                + (stat.SpecialTD + stat.DefensiveTD) * _scoring.PointsPerTouchdown
                                + stat.Safties * _scoring.PointsPerSafety;

            return new WeeklyFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Week = stat.Week,
                Games = stat.Games,
                FantasyPoints = fantasyPoints,
                Name = stat.Name,
                Position = PositionEnum.DST.ToString()
            };
        }


    }
}
