﻿using Football.Projections.Models;

namespace Football.Projections.Interfaces
{
    public interface IAdjustmentService
    {
        public Task<List<SeasonProjection>> AdjustmentEngine(List<SeasonProjection> seasonProjections);
    }
}
