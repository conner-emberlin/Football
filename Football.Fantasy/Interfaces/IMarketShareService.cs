using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMarketShareService
    {
        public Task<List<MarketShare>> GetMarketShare(PositionEnum position);
        public Task<List<TeamTotals>> GetTeamTotals();

    }
}
