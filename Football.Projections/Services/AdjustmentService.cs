using Football.Models;
using Football.Players.Interfaces;
using Football.Projections.Models;
using Football.Projections.Interfaces;
using Serilog;
using Microsoft.Extensions.Options;

namespace Football.Projections.Services
{
    public class AdjustmentService : IAdjustmentService
    {
        private readonly IPlayersService _playersService;
        private readonly ILogger _logger;
        private readonly Season _season;
        private readonly Tunings _tunings;

        public AdjustmentService(IPlayersService playerService, ILogger logger, IOptionsMonitor<Season> season, IOptionsMonitor<Tunings> tunings)
        {
            _playersService = playerService;
            _logger = logger;
            _season = season.CurrentValue;
            _tunings = tunings.CurrentValue;
        }

        public async Task<List<SeasonProjection>> AdjustmentEngine(List<SeasonProjection> seasonProjections)
        {
            _logger.Information("Calculating adjustments");
            seasonProjections = await InjuryAdjustment(seasonProjections);
            seasonProjections = await SuspensionAdjustment(seasonProjections);

            var position = seasonProjections.First().Position;
            if (position == "WR")
            {
                seasonProjections = await QuarterbackChangeAdjustment(seasonProjections);
            }

            return seasonProjections;
        }

        private async Task<List<SeasonProjection>> InjuryAdjustment(List<SeasonProjection> seasonProjections)
        {
            foreach(var s in seasonProjections)
            {
                var gamesInjured = await _playersService.GetPlayerInjuries(s.PlayerId, _season.CurrentSeason);
                if ( gamesInjured > 0)
                {
                    _logger.Information("Injury adjustment of {p} days for player {t}: {v}", gamesInjured, s.PlayerId, s.Name);
                    s.ProjectedPoints -= (s.ProjectedPoints / _season.Games) * gamesInjured;
                }
            }
            return seasonProjections;
        }

        private async Task<List<SeasonProjection>> SuspensionAdjustment(List<SeasonProjection> seasonProjections)
        {
            foreach (var s in seasonProjections)
            {
                var gamesSuspended = await _playersService.GetPlayerSuspensions(s.PlayerId, _season.CurrentSeason);
                if (gamesSuspended > 0)
                {
                    _logger.Information("Suspension adjustment of {p} days for player {t}: {v}", gamesSuspended, s.PlayerId, s.Name);
                    s.ProjectedPoints -= (s.ProjectedPoints / _season.Games) * gamesSuspended;
                }
            }
            return seasonProjections;
        }
        private async Task<List<SeasonProjection>> QuarterbackChangeAdjustment(List<SeasonProjection> seasonProjections)
        {
            var qbChanges = await _playersService.GetQuarterbackChanges(_season.CurrentSeason);
            foreach(var s in seasonProjections)
            {
                if (qbChanges.Any(q => q.PlayerId == s.PlayerId))
                {                   
                    var changeRecord = qbChanges.Where(q => q.PlayerId == s.PlayerId).First();
                    _logger.Information("QB Change found for player {p}: {n}. The New QB is {q}.", s.PlayerId, s.Name, changeRecord.CurrentQB);
                    var previousEPA = await _playersService.GetEPA(changeRecord.PreviousQB, _season.CurrentSeason - 1);
                    var currentEPA = await _playersService.GetEPA(changeRecord.CurrentQB, _season.CurrentSeason - 1);
                    var ratio = EPARatio(previousEPA, currentEPA);
                    s.ProjectedPoints *= ratio;
                }
            }
            return seasonProjections;
        }

        private double EPARatio(double previousEPA, double currentEPA)
        {
            if (previousEPA == 0)
            {
                return 1;
            }
            else if(previousEPA > currentEPA)
            {
                return Math.Max(_tunings.NewQBFloor, currentEPA / previousEPA);
            }
            else
            {
                return Math.Min(_tunings.NewQBCeiling, currentEPA / previousEPA);
            }
        }
        
    }
}
