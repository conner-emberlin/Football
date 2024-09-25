using Football.Players.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadWeeklyDataRepository
    {
        public Task<int> UploadWeeklyQBData(IEnumerable<WeeklyDataQB> players);
        public Task<int> UploadWeeklyRBData(IEnumerable<WeeklyDataRB> players);
        public Task<int> UploadWeeklyWRData(IEnumerable<WeeklyDataWR> players);
        public Task<int> UploadWeeklyTEData(IEnumerable<WeeklyDataTE> players);
        public Task<int> UploadWeeklyDSTData(List<WeeklyDataDST> players);
        public Task<int> UploadWeeklyKData(List<WeeklyDataK> players);
        public Task<int> UploadWeeklyGameResults(List<GameResult> results);
        public Task<int> UploadWeeklyRosterPercentages(IEnumerable<WeeklyRosterPercent> rosterPercentages);
        public Task<int> UploadWeeklySnapCounts(List<SnapCount> snapCounts);
        public Task<int> UploadWeeklyRedZoneRB(IEnumerable<WeeklyRedZoneRB> players);
        public Task<int> UploadConsensusWeeklyProjections(IEnumerable<ConsensusWeeklyProjections> projections);
        public Task<int> UploadPlayerTeamsInSeason(IEnumerable<PlayerTeam> playerTeams);
    }
}
