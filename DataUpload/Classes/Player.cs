using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace DataUpload.Classes
{
    [Delimiter(",")]
    public class Player
    {
        [Name("Position")]
        public string Position { get; set; }
        [Name("Team")]

        public string Team { get; set; }
        [Name("Name")]
        public string Name { get; set; }

        

        

    }
}
