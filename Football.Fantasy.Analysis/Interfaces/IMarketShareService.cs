using Football.Enums;
using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IMarketShareService
    {
        public Task<List<MarketShare>> GetMarketShare(Position position);
        public Task<List<TargetShare>> GetTargetShares();
        public Task<TargetShare> GetTargetShare(int playerId);
        public Task<List<TeamTotals>> GetTeamTotals();

    }
}
