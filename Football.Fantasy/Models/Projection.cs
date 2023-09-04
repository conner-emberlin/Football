using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Fantasy.Models
{
    public class SeasonProjection
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public double ProjectedPoints { get; set; }
    }
}
