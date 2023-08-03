using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCalculations.Models
{
    public class PassingAnalysis
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double YPA { get; set; }
        public double CompletionPercentage { get; set; }
        public double PasserRating { get; set; }

        public double YPG { get; set; }

        public double FantasyPPG { get; set; }

        public double FantasyPPA { get; set; }

    }
}
