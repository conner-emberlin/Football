﻿using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMatchupAnalysisService
    {
        public Task<List<MatchupRanking>> PositionalMatchupRankings(string position);
    }
}
