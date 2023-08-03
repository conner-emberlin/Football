using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFLData.Models;


namespace DataAnalysis.Models
{
    public class FantasyRegression1
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
        public int FantasyPoints { get; set; }
        public double GamesPlayed { get; set; }
        public double PassAttempts { get; set; }
        public double RbRushAttempts { get; set; }
        public double WrTargets { get; set; }
        public double TeTargets { get; set; }


    }
}
