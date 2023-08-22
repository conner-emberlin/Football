using Football.Interfaces;
using Football.Models;
using Football.Repository;
using Serilog;

namespace Football.Services
{
    public  class FantasyService : IFantasyService
    {
        private readonly IFantasyRepository _fantasyRepository;
        private readonly ILogger _logger;

        private readonly int pointsPerReception = 1;
        private readonly int pointsPerPassingTouchdown = 6;
        private readonly int pointsPerInterception = 2;
        private readonly int pointsPerFumble = 2;
        public FantasyService(IFantasyRepository fantasyRepository, ILogger logger)
        {
            _fantasyRepository = fantasyRepository;
            _logger = logger;
        }
        public async Task<double> CalculateTotalPoints(int playerId, int season)
        {
            return await CalculatePassingPoints(playerId, season) + await CalculateRushingPoints(playerId, season)
                    + await CalculateReceivingPoints(playerId, season);
        }

        public async Task<double> CalculatePassingPoints(int playerId, int season)
        {
            try
            {
                var fantasyPassing = await _fantasyRepository.GetFantasyPassing(playerId, season);
                return fantasyPassing != null ?
                    fantasyPassing.Yards * 0.04 + fantasyPassing.Touchdowns * pointsPerPassingTouchdown - fantasyPassing.Interceptions * pointsPerInterception
                    : 0;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task<double> CalculateRushingPoints(int playerId, int season)
        {
            try
            {
                var fantasyRushing = await _fantasyRepository.GetFantasyRushing(playerId, season);
                return fantasyRushing != null ?
                    fantasyRushing.Yards * 0.1 + fantasyRushing.Touchdowns * 6 - fantasyRushing.Fumbles * pointsPerFumble
                    : 0;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task<double> CalculateReceivingPoints(int playerId, int season)
        {
            try
            {
                var fantasyReceiving = await _fantasyRepository.GetFantasyReceiving(playerId, season);
                return fantasyReceiving != null ?
                    fantasyReceiving.Yards * 0.1 + fantasyReceiving.Touchdowns * 6 + fantasyReceiving.Receptions * pointsPerReception
                    : 0;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task<FantasyPoints> GetFantasyPoints(int playerId, int season)
        {
            return new FantasyPoints
            {
                Season = season,
                PlayerId = playerId,
                TotalPoints = Math.Round((double)await CalculateTotalPoints(playerId, season)),
                PassingPoints =Math.Round((double)await CalculatePassingPoints(playerId, season)),
                RushingPoints = Math.Round((double)await CalculateRushingPoints(playerId, season)),
                ReceivingPoints = Math.Round((double)await CalculateReceivingPoints(playerId, season))
            };
        }

        public async Task<FantasyPoints> GetFantasyResults(int playerId, int season)
        {
            return await _fantasyRepository.GetFantasyResults(playerId, season);
        }
        public async Task<List<FantasyPoints>> GetAllFantasyResults(int playerId)
        {
            return await _fantasyRepository.GetAllFantasyResults(playerId);
        }
        public async Task<int> InsertFantasyPoints(FantasyPoints fantasyPoints)
        {
            return await _fantasyRepository.InsertFantasyPoints(fantasyPoints);
        }

        public async Task<int> RefreshFantasyResults(FantasyPoints fantasyPoints)
        {
            return await _fantasyRepository.RefreshFantasyResults(fantasyPoints);
        }

        public async Task<int> InsertFantasyProjections(int rank, ProjectionModel proj)
        {
            return await _fantasyRepository.InsertFantasyProjections(rank, proj);
        }
    }
}
