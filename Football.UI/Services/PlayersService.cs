using Football.Api.Models.Players;
using Football.UI.Interfaces;

namespace Football.UI.Services
{
    public class PlayersService(IRequests requests) : IPlayersService
    {
        public async Task<PlayerDataModel?> GetPlayerDataRequest(int playerId) => await requests.Get<PlayerDataModel?>("/player/" + playerId.ToString());
        public async Task<List<TrendingPlayerModel>?> GetTrendingPlayersRequest() => await requests.Get<List<TrendingPlayerModel>?>("/player/trending-players");
        public async Task<bool> CreateRookieRequest(RookiePlayerModel rookie) => await requests.Post<bool, RookiePlayerModel>("/player/add-rookie", rookie);
        public async Task<List<SnapCountModel>?> GetSnapCountsRequest(string id) => await requests.Get<List<SnapCountModel>?>("/player/snap-counts/" + id);
        public async Task<List<SimplePlayerModel>?> GetSimplePlayersRequest(bool activeOnly = false)
        {
            var path = "/player/data/players";
            if (activeOnly) path += string.Format("?active=1");
            return await requests.Get<List<SimplePlayerModel>?>(path);
        }
    }
}
