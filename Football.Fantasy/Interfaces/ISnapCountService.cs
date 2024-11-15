using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface ISnapCountService
    {
        Task<List<SnapCountAnalysis>> GetSnapCountAnalysis(Position position, int season);
        Task<List<SnapCountSplit>> GetSnapCountSplits(IEnumerable<int> playerIds, int season);
    }
}
