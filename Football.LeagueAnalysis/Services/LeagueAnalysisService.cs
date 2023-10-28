using Football.LeagueAnalysis.Interfaces;
using Football.LeagueAnalysis.Models;

namespace Football.LeagueAnalysis.Services
{
    public class LeagueAnalysisService : ILeagueAnalysisService
    {
        private readonly ILeagueAnalysisRepository _leagueAnalysisRepository;
        private readonly ISleeperLeagueService _sleeperLeagueService;

        public LeagueAnalysisService(ILeagueAnalysisRepository leagueAnalysisRepository, ISleeperLeagueService sleeperLeagueService)
        {
            _leagueAnalysisRepository = leagueAnalysisRepository;
            _sleeperLeagueService = sleeperLeagueService;
        }

        public async Task<int> UploadSleeperPlayerMap()
        {
            var sleeperPlayers =  await _sleeperLeagueService.GetSleeperPlayers();
            var playerMap = await _sleeperLeagueService.GetSleeperPlayerMap(sleeperPlayers);
            return await _leagueAnalysisRepository.UploadSleeperPlayerMap(playerMap);
        }
    }
}
