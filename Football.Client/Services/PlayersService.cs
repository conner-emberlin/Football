using Football.Shared.Models.Players;
using Football.Shared.Models.Fantasy;

using Football.Client.Interfaces;

namespace Football.Client.Services
{
    public class PlayersService(IRequests requests) : IPlayersService
    {
        public async Task<PlayerDataModel?> GetPlayerDataRequest(int playerId) => await requests.Get<PlayerDataModel?>("/player/" + playerId.ToString());
        public async Task<SimplePlayerModel?> GetSimplePlayerRequest(string playerId) => await requests.Get<SimplePlayerModel?>("/player/simple/" + playerId);
        public async Task<List<TrendingPlayerModel>?> GetTrendingPlayersRequest() => await requests.Get<List<TrendingPlayerModel>?>("/player/trending-players");
        public async Task<bool> CreateRookieRequest(RookiePlayerModel rookie) => await requests.Post<bool, RookiePlayerModel>("/player/add-rookie", rookie);
        public async Task<List<SnapCountModel>?> GetSnapCountsRequest(string id) => await requests.Get<List<SnapCountModel>?>("/player/snap-counts/" + id);
        public async Task<List<RookiePlayerModel>?> GetAllRookiesRequest() => await requests.Get<List<RookiePlayerModel>?>("/player/rookies/all");
        public async Task<int> InactivatePlayersRequest(List<int> playerIds) => await requests.Post<int, List<int>>("/player/inactivate", playerIds);
        public async Task<int> PostInSeasonInjuryRequest(InSeasonInjuryModel injury) => await requests.Post<int, InSeasonInjuryModel>("/player/injury", injury);
        public async Task<List<PlayerInjuryModel>?> GetAllInSeasonInjuriesRequest() => await requests.Get<List<PlayerInjuryModel>?>("/player/player-injuries");
        public async Task<bool> UpdateInSeasonInjuryRequest(InSeasonInjuryModel injury) => await requests.PutWithBody<bool, InSeasonInjuryModel>("/player/update-injury", injury);
        public async Task<List<InSeasonTeamChangeModel>?> GetInSeasonTeamChangesRequest() => await requests.Get<List<InSeasonTeamChangeModel>?>("/player/team-changes");
        public async Task<int> PostInSeasonTeamChangesRequest(InSeasonTeamChangeModel change) => await requests.Post<int, InSeasonTeamChangeModel>("/player/team-change/in-season", change);
        public async Task<List<int>?> GetWeeklyDataSeasonsRequest() => await requests.Get<List<int>?>("/player/weekly-data-seasons");
        public async Task<List<WeeklyDataModel>?> GetWeeklyDataByPlayerRequest(string position, string playerId, string season) => await requests.Get<List<WeeklyDataModel>?>(string.Format("/player/weekly-data/{0}/{1}/{2}", position, playerId, season));
        public async Task<List<WeeklyFantasyModel>?> GetWeeklyFantasyByPlayerRequest(string playerId, string season) => await requests.Get<List<WeeklyFantasyModel>?>(string.Format("/player/weekly-fantasy/{0}/{1}", playerId, season));
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
