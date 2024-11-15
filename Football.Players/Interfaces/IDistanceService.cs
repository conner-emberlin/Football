
namespace Football.Players.Interfaces
{
    public interface IDistanceService
    {
        Task<double> GetTravelDistance(int playerId);
    }
}
