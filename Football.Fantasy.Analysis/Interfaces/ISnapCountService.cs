using Football.Enums;
using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface ISnapCountService
    {
        public Task<List<SnapCountAnalysis>> GetSnapCountAnalysis(Position position, int season);
    }
}
