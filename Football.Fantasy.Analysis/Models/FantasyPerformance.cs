using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Fantasy.Analysis.Models
{
    public class FantasyPerformance
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public int Season { get; set; }
        public double TotalFantasy { get; set; }
        public double AvgFantasy { get; set; }
        public double MinFantasy { get; set; }
        public double MaxFantasy { get;set; }
        public double Variance { get; set; } 
        public double StdDev { get; set; }
    }
}
