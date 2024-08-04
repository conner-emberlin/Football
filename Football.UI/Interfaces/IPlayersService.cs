using Football.Shared.Models.Players;

namespace Football.UI.Interfaces
{
    public interface IPlayersService
    {
        public Task<bool> CreateRookieRequest(RookiePlayerModel rookie);
        public Task<PlayerDataModel?> GetPlayerDataRequest(int playerId);
        public Task<List<TrendingPlayerModel>?> GetTrendingPlayersRequest();
        public Task<List<SimplePlayerModel>?> GetSimplePlayersRequest(bool activeOnly = false, string position = "");
        public Task<List<SnapCountModel>?> GetSnapCountsRequest(string id);
        public Task<List<RookiePlayerModel>?> GetAllRookiesRequest();
        public Task<int> InactivatePlayersRequest(List<int> playerIds);
        public Task<int> PostInSeasonInjuryRequest(InSeasonInjuryModel injury);
        public Task<List<PlayerInjuryModel>?> GetAllInSeasonInjuriesRequest();
        public Task<bool> UpdateInSeasonInjuryRequest(InSeasonInjuryModel injury);
    }
}
