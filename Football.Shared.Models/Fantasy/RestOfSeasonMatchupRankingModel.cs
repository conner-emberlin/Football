using Football.Shared.Models.Teams;

namespace Football.Shared.Models.Fantasy
{
    public class RestOfSeasonMatchupRankingModel
    {
        public TeamMapModel TeamMapModel { get; set; } = new();
        public Dictionary<string, List<MatchupRankingModel>> Rankings = [];
    }
}
