﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Players.Models
{
    public class Suspensions
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Length { get; set; }
    }
}
