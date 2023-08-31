using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Models
{
    public class Season
    {
        public int CurrentSeason { get; set; }
        public int Games { get; set; }
    }

    public class ReplacementLevels
    {
        public int ReplacementLevelQB { get; set; }
        public int ReplacementLevelRB { get; set; }
        public int ReplacementLevelWR { get; set; }
        public int ReplacementLevelTE { get; set; }
    }

    public class FantasyScoring
    {
        public int PointsPerReception { get; set; }
        public int PointsPerPassingTouchdown { get; set; }
        public int PointsPerInterception { get; set; }
        public int PointsPerFumble { get; set; }
        public int PointsPerTouchdown { get; set; }
        public double PointsPerPassingYard { get; set; }
        public double PointsPerYard { get; set; }
    }

    public class Projections
    {
        public int QBProjections { get; set; }
        public int RBProjections { get; set; }
        public int WRProjections { get; set; }
        public int TEProjections { get; set; }
    }

    public class Starters
    {
        public int QBStarters { get; set; }
        public int RBStarters { get; set; }
        public int WRStarters { get; set; }
        public int TEStarters { get; set; }
    }
    public class Tunings
    {
        public int RBFloor { get; set; }
        public double LeadRBFactor { get; set; }
        public double Weight { get; set; }
        public double SecondYearWRLeap { get; set; }
        public double SecondYearRBLeap { get; set; }
        public double SecondYearQBLeap { get; set; }
    }
}
