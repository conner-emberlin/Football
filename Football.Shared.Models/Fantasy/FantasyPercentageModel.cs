﻿namespace Football.Shared.Models.Fantasy
{
    public class FantasyPercentageModel
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double TotalPoints { get; set; }
        public double PassYDShare { get; set; }
        public double PassTDShare { get; set; }
        public double RushYDShare { get; set; }
        public double RushTDShare { get; set; }
        public double RecShare { get; set; }
        public double RecYDShare { get; set; }
        public double RecTDShare { get; set; }
    }
}
