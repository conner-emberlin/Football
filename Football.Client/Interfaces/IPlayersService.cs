using Football.Shared.Models.Players;

namespace Football.Client.Interfaces
{
    public interface IPlayersService
    {
        Task<bool> CreateRookieRequest(RookiePlayerModel rookie);
        Task<PlayerDataModel?> GetPlayerDataRequest(int playerId);
        Task<List<TrendingPlayerModel>?> GetTrendingPlayersRequest();
        Task<List<SimplePlayerModel>?> GetSimplePlayersRequest(bool activeOnly = false, string position = "");
        Task<SimplePlayerModel?> GetSimplePlayerRequest(string playerId);
        Task<List<SnapCountModel>?> GetSnapCountsRequest(string id);
        Task<List<RookiePlayerModel>?> GetAllRookiesRequest();
        Task<int> InactivatePlayersRequest(List<int> playerIds);
        Task<int> PostInSeasonInjuryRequest(InSeasonInjuryModel injury);
        Task<List<PlayerInjuryModel>?> GetAllInSeasonInjuriesRequest();
        Task<bool> UpdateInSeasonInjuryRequest(InSeasonInjuryModel injury);
        Task<List<InSeasonTeamChangeModel>?> GetInSeasonTeamChangesRequest();
        Task<int> PostInSeasonTeamChangesRequest(InSeasonTeamChangeModel change);
    }
}
