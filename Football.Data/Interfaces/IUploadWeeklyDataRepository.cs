using Football.Players.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadWeeklyDataRepository
    {
        Task<int> UploadWeeklyQBData(IEnumerable<WeeklyDataQB> players);
        Task<int> UploadWeeklyRBData(IEnumerable<WeeklyDataRB> players);
        Task<int> UploadWeeklyWRData(IEnumerable<WeeklyDataWR> players);
        Task<int> UploadWeeklyTEData(IEnumerable<WeeklyDataTE> players);
        Task<int> UploadWeeklyDSTData(List<WeeklyDataDST> players);
        Task<int> UploadWeeklyKData(List<WeeklyDataK> players);
        Task<int> UploadWeeklyGameResults(List<GameResult> results);
        Task<int> UploadWeeklyRosterPercentages(IEnumerable<WeeklyRosterPercent> rosterPercentages);
        Task<int> UploadWeeklySnapCounts(List<SnapCount> snapCounts);
        Task<int> UploadWeeklyRedZoneRB(IEnumerable<WeeklyRedZoneRB> players);
        Task<int> UploadConsensusWeeklyProjections(IEnumerable<ConsensusWeeklyProjections> projections);
        Task<int> UploadPlayerTeamsInSeason(IEnumerable<PlayerTeam> playerTeams);
    }
}
