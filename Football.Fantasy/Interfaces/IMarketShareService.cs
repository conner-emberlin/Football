using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMarketShareService
    {
        public Task<List<TeamTotals>> GetTeamTotals();

    }
}
