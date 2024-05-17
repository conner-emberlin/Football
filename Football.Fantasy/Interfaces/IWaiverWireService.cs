using Football.Data.Models;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IWaiverWireService
    {
        public Task<List<WaiverWireCandidate>> GetWaiverWireCandidates(int season, int week);
    }
}
