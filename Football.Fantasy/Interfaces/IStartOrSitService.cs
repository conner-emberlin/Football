using Football.Fantasy.Models;

namespace Football.Fantasy.Interfaces
{
    public interface IStartOrSitService
    {
        Task<List<StartOrSit>> GetStartOrSits(List<int> playerIds);

    }
}
