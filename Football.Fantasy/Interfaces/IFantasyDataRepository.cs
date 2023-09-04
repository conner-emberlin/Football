using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataRepository
    {
        public Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season);
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId);
        public Task<int> PostSeasonFantasy(SeasonFantasy data);
        public Task<List<Player>> GetPlayersByPosition(string position);
        public Task<Player> GetPlayer(int playerId);
    }
}
