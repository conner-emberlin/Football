using Football.Enums;
using Football.Players.Models;
using Football.Fantasy.Analysis.Models;

namespace Football.Fantasy.Analysis.Interfaces
{
    public interface IBoomBustService
    {
        public Task<List<BoomBust>> GetBoomBusts(Position position);
        public Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId);
    }
}
