using Football.Enums;
using Football.Fantasy.Models;
using Football.Players.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMarketShareService
    {
        public Task<List<MarketShare>> GetMarketShare(PositionEnum position);
        public Task<TargetShare> GetTargetShare(int teamId);
        public Task<List<TeamTotals>> GetTeamTotals();

    }
}
