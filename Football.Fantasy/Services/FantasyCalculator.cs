﻿using Football.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Players.Models;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class FantasyCalculator(IOptionsMonitor<FantasyScoring> scoring, IMapper mapper) : IFantasyCalculator
    {
        private readonly FantasyScoring _scoring = scoring.CurrentValue;

        public SeasonFantasy CalculateFantasy(SeasonDataQB stat)
        {
            var seasonFantasy = mapper.Map<SeasonFantasy>(stat);
            seasonFantasy.FantasyPoints = _scoring.PointsPerPassingTouchdown * stat.TD + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerPassingYard * stat.Yards + _scoring.PointsPerYard * stat.RushingYards
                        - _scoring.PointsPerFumble * stat.Fumbles - _scoring.PointsPerInterception * stat.Int;
            return seasonFantasy;
        }
        public WeeklyFantasy CalculateFantasy(WeeklyDataQB stat)
        {
            var weeklyFantasy = mapper.Map<WeeklyFantasy>(stat);
            weeklyFantasy.FantasyPoints = _scoring.PointsPerPassingTouchdown * stat.TD + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerPassingYard * stat.Yards + _scoring.PointsPerYard * stat.RushingYards
                        - _scoring.PointsPerFumble * stat.Fumbles - _scoring.PointsPerInterception * stat.Int;
            return weeklyFantasy;
        }
        public SeasonFantasy CalculateFantasy(SeasonDataRB stat)
        {
            var seasonFantasy = mapper.Map<SeasonFantasy>(stat);
            seasonFantasy.FantasyPoints = _scoring.PointsPerYard * stat.RushingYds + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerTouchdown * stat.ReceivingTD
                        + _scoring.PointsPerYard * stat.Yards - _scoring.PointsPerFumble * stat.Fumbles;
            return seasonFantasy;
        }
        public WeeklyFantasy CalculateFantasy(WeeklyDataRB stat)
        {
            var weeklyFantasy = mapper.Map<WeeklyFantasy>(stat);
            weeklyFantasy.FantasyPoints = _scoring.PointsPerYard * stat.RushingYds + _scoring.PointsPerTouchdown * stat.RushingTD
                        + _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerTouchdown * stat.ReceivingTD
                        + _scoring.PointsPerYard * stat.Yards - _scoring.PointsPerFumble * stat.Fumbles;
            return weeklyFantasy;
        }
        public SeasonFantasy CalculateFantasy(SeasonDataWR stat)
        {
            var seasonFantasy = mapper.Map<SeasonFantasy>(stat);
            seasonFantasy.FantasyPoints = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return seasonFantasy;
        }
        public WeeklyFantasy CalculateFantasy(WeeklyDataWR stat)
        {
            var weeklyFantasy = mapper.Map<WeeklyFantasy>(stat);
            weeklyFantasy.FantasyPoints = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return weeklyFantasy;
        }
        public SeasonFantasy CalculateFantasy(SeasonDataTE stat)
        {
            var seasonFantasy = mapper.Map<SeasonFantasy>(stat);
            seasonFantasy.FantasyPoints = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return seasonFantasy;
        }
        public WeeklyFantasy CalculateFantasy(WeeklyDataTE stat)
        {
            var weeklyFantasy = mapper.Map<WeeklyFantasy>(stat);
            weeklyFantasy.FantasyPoints = _scoring.PointsPerReception * stat.Receptions + _scoring.PointsPerYard * stat.Yards
                        + _scoring.PointsPerTouchdown * stat.TD + _scoring.PointsPerYard * stat.RushingYds
                        + _scoring.PointsPerTouchdown * stat.RushingTD - _scoring.PointsPerFumble * stat.Fumbles;
            return weeklyFantasy;
        }
        public WeeklyFantasy CalculateFantasy(WeeklyDataDST stat, WeeklyDataDST opponentStat, GameResult result, int teamId)
        {
            var weeklyFantasy = mapper.Map<WeeklyFantasy>(stat);
            var pointsAllowed = result.WinnerId == teamId ? result.LoserPoints - _scoring.PointsPerTouchdown * opponentStat.DefensiveTD 
                                : result.WinnerPoints - _scoring.PointsPerTouchdown * opponentStat.DefensiveTD;

            var pointsAllowedFantasy = pointsAllowed == 0 ? _scoring.ZeroPointsAllowed
                                     : 1 <= pointsAllowed && pointsAllowed <= 6 ? _scoring.OneToSixPointsAllowed
                                     : 7 <= pointsAllowed && pointsAllowed <= 13 ? _scoring.SevenToThirteenPointsAllowed
                                     : 14 <= pointsAllowed && pointsAllowed <= 17 ? _scoring.FourteenToSeventeenPointsAllowed
                                     : 18 <= pointsAllowed && pointsAllowed <= 27 ? _scoring.EighteenToTwentySevenPointsAllowed
                                     : 28 <= pointsAllowed && pointsAllowed <= 34 ? _scoring.TwentyEightToThirtyFourPointsAllowed
                                     : 35 <= pointsAllowed && pointsAllowed <= 45 ? _scoring.ThirtyFiveToFourtyFivePointsAllowed
                                     : _scoring.FourtySixOrMorePointsAllowed;

            weeklyFantasy.FantasyPoints = pointsAllowedFantasy
                                        + stat.Sacks * _scoring.PointsPerSack
                                        + stat.Ints * _scoring.PointsPerInterception
                                        + stat.FumblesRecovered * _scoring.PointsPerFumble
                                        + (stat.SpecialTD + stat.DefensiveTD) * _scoring.PointsPerTouchdown
                                        + stat.Safties * _scoring.PointsPerSafety;

            return weeklyFantasy;
        }

        public WeeklyFantasy CalculateFantasy(WeeklyDataK stat)
        {
            var weeklyFantasy = mapper.Map<WeeklyFantasy>(stat);
            weeklyFantasy.FantasyPoints = stat.ExtraPoints * _scoring.ExtraPoint
                         + (stat.ExtraPointAttempts - stat.ExtraPoints) * _scoring.ExtraPointMissed
                         + (stat.FieldGoalAttempts - stat.FieldGoals) * _scoring.FGMissed
                         + (stat.OneNineteen + stat.TwentyTwentyNine + stat.ThirtyThirtyNine) * _scoring.FGLessThanFourty
                         + stat.FourtyFourtyNine * _scoring.FGFourtyFifty
                         + stat.Fifty * _scoring.FGGreaterThanFifty;
            return weeklyFantasy;
        }


    }
}
