using Football.Data.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IWaiverWireService
    {
        public Task<List<WeeklyRosterPercent>> GetWaiverWireCandidates(int season, int week);
    }
}
