using Football.Players.Interfaces;
using Football.Statistics.Interfaces;
using Football.Enums;
using Football.Data.Models;
using Football.Models;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using Football.Statistics.Models;

namespace Football.Statistics.Services
{
    public class AdvancedStatisticsService : IAdvancedStatisticsService
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IPlayersService _playersService;
        private readonly FiveThirtyEightValueSettings _value;

        public AdvancedStatisticsService(IStatisticsService statisticsService, IPlayersService playersService, IOptionsMonitor<FiveThirtyEightValueSettings> value)
        {
            _statisticsService = statisticsService;
            _playersService = playersService;
            _value = value.CurrentValue;
        }

        public async Task<List<QBValue>> PasserRating()
        {
            List<QBValue> values = new();
            var qbs = await _playersService.GetPlayersByPosition(Position.QB);
            foreach (var qb in qbs)
            {
                var value = await PasserRating(qb.PlayerId);
                if (value > 0)
                {
                    values.Add(new QBValue
                    {
                        PlayerId = qb.PlayerId,
                        Name = qb.Name,
                        Value = value
                    });
                }
            }
            return values.OrderByDescending(v => v.Value).ToList();
        }
        public async Task<List<QBValue>> FiveThirtyEightQBValue()
        {
            List<QBValue> values = new();
            var qbs = await _playersService.GetPlayersByPosition(Position.QB);
            foreach (var qb in qbs)
            {
                var value = await FiveThirtyEightQBValue(qb.PlayerId);
                if (value > 0)
                {
                    values.Add(new QBValue
                    {
                        PlayerId = qb.PlayerId,
                        Name = qb.Name,
                        Value = value
                    });
                }
            }            
            return values.OrderByDescending(v => v.Value).ToList();
        }
        public async Task<double> FiveThirtyEightQBValue(int playerId)
        {
            var player = await _playersService.GetPlayer(playerId);
            if (Enum.TryParse(player.Position, out Position position) && position == Position.QB)
            {
                var weeklyStats = await _statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId);
                if (weeklyStats.Any())
                {
                    return Math.Round(
                             weeklyStats.Average(w => w.Attempts) * _value.PAtts
                           + weeklyStats.Average(w => w.Completions) * _value.PComps
                           + weeklyStats.Average(w => w.TD) * _value.PTds
                           + weeklyStats.Average(w => w.Int) * _value.Ints
                           + weeklyStats.Average(w => w.Sacks) * _value.Sacks
                           + weeklyStats.Average(w => w.RushingAttempts) * _value.RAtts
                           + weeklyStats.Average(w => w.RushingYards) * _value.RYds
                           + weeklyStats.Average(w => w.RushingTD) * _value.RTds);
                }
                else return 0;
            }
            else return 0;
        }

        public async Task<double> PasserRating(int playerId)
        {
            var player = await _playersService.GetPlayer(playerId);
            if (Enum.TryParse(player.Position, out Position position) && position == Position.QB)
            {
                var weeklyStats = await _statisticsService.GetWeeklyData<WeeklyDataQB>(Position.QB, playerId);
                if (weeklyStats.Any())
                {
                    var aActual = (weeklyStats.Average(w => w.Completions) / weeklyStats.Average(w => w.Attempts) - 0.3) * 5;
                    var bActual = (weeklyStats.Average(w => w.Yards) / weeklyStats.Average(w => w.Attempts) - 3) * 0.25;
                    var cActual = (weeklyStats.Average(w => w.TD) / weeklyStats.Average(w => w.TD)) * 20;
                    var dActual = 2.375 - (weeklyStats.Average(w => w.Int) / (weeklyStats.Average(w => w.Attempts)) * 25);

                    var a = aActual > 2.375 ? 2.375 : aActual < 0 || double.IsNaN(aActual) ? 0 : aActual;
                    var b = bActual > 2.375 ? 2.375 : bActual < 0 || double.IsNaN(bActual) ? 0 : bActual;
                    var c = cActual > 2.375 ? 2.375 : cActual < 0 || double.IsNaN(cActual) ? 0 : cActual;
                    var d = dActual > 2.375 ? 2.375 : dActual < 0 || double.IsNaN(dActual) ? 0 : dActual;

                    return ((a + b + c + d) / 6) * 100;
                }
                else return 0;

            }
            else return 0;
        }
    }
}
