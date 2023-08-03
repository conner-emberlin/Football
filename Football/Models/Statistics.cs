using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Models
{
    public class PassingStatistic
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public double Age { get; set; }
        public double Games { get; set; }
        public double Completions { get; set; }
        public double Attempts { get; set; }
        public double Yards { get; set; }
        public double Touchdowns { get; set; }
        public double Interceptions { get; set; }
        public double FirstDowns { get; set; }
        public double Long { get; set; }
        public double Sacks { get; set; }
        public double SackYards { get; set; }

    }

    public class ReceivingStatistic
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public double Age { get; set; }
        public double Games { get; set; }
        public double Targets { get; set; }
        public double Receptions { get; set; }
        public double Yards { get; set; }
        public double Touchdowns { get; set; }
        public double FirstDowns { get; set; }
        public double Long { get; set; }
        public double RpG { get; set; }
        public double Fumbles { get; set; }
    }

    public class RushingStatistic
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public double Age { get; set; }
        public double Games { get; set; }
        public double RushAttempts { get; set; }
        public double Yards { get; set; }
        public double Touchdowns { get; set; }
        public double FirstDowns { get; set; }
        public double Long { get; set; }
        public double Fumbles { get; set; }
    }
}
