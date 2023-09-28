using Football.Enums;
using Football.Models;
using Football.Players.Interfaces;
using Football.Data.Models;
using Football.Players.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IMarketShareService
    {
        public Task<List<SeasonDataQB>> GetMarketShareQB(List<Player> players);
        public Task<List<SeasonDataRB>> GetMarketShareRB(List<Player> players);
        public Task<List<SeasonDataWR>> GetMarketShareWR(List<Player> players);
        public Task<List<SeasonDataTE>> GetMarketShareTE(List<Player> players);
    }
}
