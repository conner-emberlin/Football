﻿using Football.Shared.Models.Projection;

namespace Football.Client.Interfaces
{
    public interface IProjectionService
    {
        public Task<List<WeeklyProjectionAnalysisModel>?> GetCurrentSeasonWeeklyAnalysisRequest(string position);
        public Task<List<SeasonProjectionAnalysisModel>?> GetPreviousSeasonsAnalysisRequest(string position);
        public Task<List<SeasonProjectionModel>?> GetSeasonProjectionsRequest(string position, List<string> filter);
        public Task<bool> DeleteSeasonProjectionRequest(int playerId, int season);
        public Task<int> PostSeasonProjectionRequest(string position, List<string> filters);
        public Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string position, string week);
        public Task<List<WeeklyProjectionErrorModel>?> GetWeeklyProjectionErrorRequest(string id);
        public Task<WeeklyProjectionAnalysisModel?> GetWeeklyProjectionAnalysisRequest(string id);
        public Task<List<WeekProjectionModel>?> GetWeekProjectionsRequest(string position, List<string> filter);
        public Task<bool> DeleteWeekProjectionRequest(int playerId, int season, int week);
        public Task<int> PostWeekProjectionRequest(string position, List<string> filters);
        public Task<List<MatchupProjectionsModel>?> GetMatchupProjectionsRequest(string teamName);
        public Task<List<string>?> GetSeasonModelVariablesRequest(string position);
        public Task<SeasonProjectionsExistModel?> GetSeasonProjectionsExistRequest(string position);
        public Task<List<string>?> GetWeeklyModelVariablesRequest(string position);
        public Task<WeeklyProjectionsExistModel?> GetWeeklyProjectionsExistRequest(string position);
        public Task<List<AdjustmentDescriptionModel>?> GetSeasonAdjustmentDescriptionsRequest();
    }
}
