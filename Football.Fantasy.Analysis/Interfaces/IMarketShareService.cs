﻿using Football.Enums;
using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IMarketShareService
    {
        public Task<List<MarketShare>> GetMarketShare(PositionEnum position);
        public Task<List<TargetShare>> GetTargetShares();
        public Task<List<TeamTotals>> GetTeamTotals();

    }
}