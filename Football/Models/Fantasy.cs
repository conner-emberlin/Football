using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Models
{
    public class Fantasy
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double Yards { get; set; }
        public double Touchdowns { get; set; }
    }

    public class FantasyPassing : Fantasy
    {
        public double Interceptions { get; set; }
    }

    public class FantasyReceiving : Fantasy
    {
        public double Receptions { get; set; }
    }

    public class FantasyRushing : Fantasy
    {
        public double Fumbles { get; set; }
    }

    public class FantasyPoints
    {
        public int Season { get; set; }
        public int PlayerId { get; set; }
        public double TotalPoints { get; set; }
        public double PassingPoints { get; set; }
        public double RushingPoints { get; set; }
        public double ReceivingPoints { get; set; }
    }
}
