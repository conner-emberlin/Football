using Football.Enums;
using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMarketShareService
    {
        Task<List<MarketShare>> GetMarketShare(Position position);
        Task<List<TargetShare>> GetTargetShares();
        Task<List<TeamTotals>> GetTeamTotals();
        Task<TeamTotals> GetTeamTotals(int teamId);

    }
}
