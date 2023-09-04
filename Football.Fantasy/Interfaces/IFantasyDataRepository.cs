using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IFantasyDataRepository
    {
        public Task<SeasonFantasy> GetSeasonFantasy(int playerId, int season);
        public Task<List<SeasonFantasy>> GetSeasonFantasy(int playerId);
        public Task<int> PostSeasonFantasy(SeasonFantasy data);
    }
}
