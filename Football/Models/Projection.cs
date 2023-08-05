using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Models
{
    public class ProjectionModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public double ProjectedPoints { get; set; }
    }
}
