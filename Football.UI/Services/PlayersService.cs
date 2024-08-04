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
        public async Task<List<RookiePlayerModel>?> GetAllRookiesRequest() => await requests.Get<List<RookiePlayerModel>?>("/player/rookies/all");
        public async Task<int> InactivatePlayersRequest(List<int> playerIds) => await requests.Post<int, List<int>>("/player/inactivate", playerIds);
        public async Task<List<SimplePlayerModel>?> GetSimplePlayersRequest(bool activeOnly = false, string position = "")
        {
            var path = "/player/data/players";

            if (activeOnly && position == "") path += string.Format("?active=1");
            else if (activeOnly && position != "") path += string.Format("?active=1&position={0}", position);
            else if (!activeOnly && position != "") path += string.Format("?position={0}", position);

            return await requests.Get<List<SimplePlayerModel>?>(path);
        }
    }
}
