using Football.Data.Models;
using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IWaiverWireService
    {
        public Task<List<WaiverWireCandidate>> GetWaiverWireCandidates(int season, int week);
    }
}
