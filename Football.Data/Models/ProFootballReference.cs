using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Data.Models
{
    public class ProFootballReferenceGameScores
    {
        public int Week { get; set; }
        public string Day { get; set; } = "";
        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public string Winner { get; set; } = "";
        public string HomeIndicator { get; set; } = "";
        public string Loser { get; set; } = "";
        public int WinnerPoints { get; set; }
        public int LoserPoints { get; set;}
        public int WinnerYards { get; set; }
        public int LoserYards { get; set; }
    }
}
