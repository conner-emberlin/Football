﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Models
{
    public class QBChange
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public string? PreviousTeam { get; set; }
        public string? CurrentTeam { get; set; }
    }
}
