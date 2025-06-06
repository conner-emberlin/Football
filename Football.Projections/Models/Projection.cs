﻿using Football.Players.Models;

namespace Football.Projections.Models
{
    public class SeasonProjection
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double ProjectedPoints { get; set; }
        public string? Adjustments { get; set; }
    }

    public class SeasonFlex : SeasonProjection
    {
        public double Vorp { get; set; }
    }

    public class WeekProjection : SeasonProjection
    {
        public int Week { get; set; }
    }

    public class WeeklyProjectionAnalysis
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public string Position { get; set; } = "";
        public int ProjectionCount { get; set; }
        public double MSE { get; set; }
        public double RSquared { get; set; }
        public double MAE { get; set; }
        public double MAPE { get; set; }
        public double AvgError { get; set; }
        public double AvgRankError { get; set; }
        public double AdjAvgError { get; set; }
    }

    public class PlayerWeeklyProjectionAnalysis
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public int ProjectionCount { get; set; }
        public double MSE { get; set; }
        public double RSquared { get; set; }
        public double MAE { get; set; }
        public double MAPE { get; set; }
        public double AvgError { get; set; }
    }

    public class WeeklyProjectionError
    {
        public int Season { get; set;}
        public int Week { get; set;}
        public string Position { get; set; } = "";
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public double FantasyPoints { get; set; }
        public double ProjectedPoints { get; set; }
        public double Error { get; set; }
    }

    public class SeasonProjectionError
    {
        public Player Player { get; set; } = new();
        public double TotalFantasy { get; set; }
        public int WeeksPlayed { get; set; }
        public double SeasonFantasyProjection { get; set; }
        public double SeasonFantasyPPGProjection { get; set; }
        public double FantasyPPG { get; set; }
    }

    public class SeasonProjectionAnalysis
    {
        public int Season { get; set; }
        public string Position { get; set; } = "";
        public double MSE { get; set; }
        public double RSquared { get; set; }
        public double AvgError { get; set; }
        public double AvgErrorPerGame { get; set; }
        public double ProjectionCount { get; set; }
        public double PPGRSquared { get; set; }
    }
}

