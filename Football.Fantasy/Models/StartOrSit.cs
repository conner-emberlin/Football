using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Fantasy.Models
{
    public class StartOrSit
    {
        public MatchLines? MatchLines { get; set; }

    }

    public class MatchLines
    {
        public double? OverUnder { get; set; }
        public double? Line { get; set; }
        public double? ImpliedTeamTotal { get; set; }
    }
}
