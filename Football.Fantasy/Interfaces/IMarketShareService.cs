using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMarketShareService
    {
        public Task<List<MarketShare>> GetMarketShare(Position position);
        public Task<List<TargetShare>> GetTargetShares();
        public Task<TargetShare> GetTargetShare(int playerId);
        public Task<List<TeamTotals>> GetTeamTotals();
        public Task<TeamTotals> GetTeamTotals(int teamId);

    }
}
