using Football.Projections.Models;


namespace Football.Projections.Interfaces
{
    public interface IProjectionRepository
    {
        public Task<int> PostSeasonProjections(SeasonProjection projections);
        public Task<SeasonProjection?> GetSeasonProjection(int playerId);
    }
}
