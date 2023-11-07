
namespace Football.Players.Interfaces
{
    public interface IDistanceService
    {
        public Task<double> GetTravelDistance(int playerId);
    }
}
