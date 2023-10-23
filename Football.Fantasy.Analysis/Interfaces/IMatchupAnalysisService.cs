﻿using Football.Enums;
using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IMatchupAnalysisService
    {
        public Task<List<MatchupRanking>> PositionalMatchupRankings(PositionEnum position);
        public Task<int> GetMatchupRanking(int playerId);
    }
}