﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Data.Models
{
    public class WeeklyScraping
    {
        public string FantasyProsBaseURL { get; set; } = "";
        public string FantasyProsXPath { get; set; } = "";
        public int CurrentWeek { get; set; }
    }
}