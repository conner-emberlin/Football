﻿namespace Football.Projections.Models
{
    public class SeasonProjection
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double ProjectedPoints { get; set; }
    }

    public class SeasonFlex : SeasonProjection
    {
        public double Vorp { get; set; }
    }

    public class WeekProjection : SeasonProjection
    {
        public int Week { get; set; }
    }
    public class WeekFlex : WeekProjection
    {
        public double Vorp { get; set; }
    }

}
