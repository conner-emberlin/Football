using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface ISnapCountService
    {
        public Task<List<SnapCountAnalysis>> GetSnapCountAnalysis(Position position, int season);
        public Task<List<SnapCountSplit>> GetSnapCountSplits(IEnumerable<int> playerIds, int season);
    }
}
