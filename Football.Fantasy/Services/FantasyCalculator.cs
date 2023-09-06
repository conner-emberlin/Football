using Football.Data.Models;
using Football.Fantasy.Interfaces;
using Football.Fantasy.Models;
using Football.Models;
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
                        + _scoring.PointsPerPassingYard * stat.Yards + _scoring.PointsPerYard * _scoring.PointsPerYard
                        - _scoring.PointsPerFumble * stat.Fumbles - _scoring.PointsPerInterception * stat.Int;
            return new SeasonFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Games = stat.Games,
                FantasyPoints = points,
                Name = stat.Name,
                Position = "QB"
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
                Position = "RB"
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
                Position = "WR"
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
                Position = "TE"
            };
        }
        public SeasonFantasy CalculateDSTFantasy(SeasonDataDST stat)
        {
            var points = 0;
            return new SeasonFantasy
            {
                PlayerId = stat.PlayerId,
                Season = stat.Season,
                Games = stat.Games,
                FantasyPoints = points,
                Name = stat.Name,
                Position = "DST"
            };
        }


    }
}
