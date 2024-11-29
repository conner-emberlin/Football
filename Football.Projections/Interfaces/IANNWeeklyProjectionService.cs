using Football.Enums;
namespace Football.Projections.Interfaces
{
    public interface IANNWeeklyProjectionService
    {
        Task<List<double>> CalculateProjections(Position position);
    }
}
