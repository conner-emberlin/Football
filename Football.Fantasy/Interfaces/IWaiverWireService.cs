using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IWaiverWireService
    {
        Task<List<WaiverWireCandidate>> GetWaiverWireCandidates(int season, int week);
    }
}
