using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IStartOrSitService
    {
        public Task<List<StartOrSit>> GetStartOrSits(List<int> playerIds);

    }
}
