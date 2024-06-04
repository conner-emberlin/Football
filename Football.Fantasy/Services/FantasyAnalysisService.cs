using Football.Enums;
using Football.Models;
using Football.Fantasy.Models;
using Football.Fantasy.Interfaces;
using Football.Players.Interfaces;
using Football.Players.Models;
using Microsoft.Extensions.Options;

namespace Football.Fantasy.Services
{
    public class FantasyAnalysisService(IOptionsMonitor<Season> season,
        IPlayersService playersService, IFantasyDataService fantasyDataService, ISettingsService settingsService,
        IStatisticsService statsService, IOptionsMonitor<FantasyScoring> scoring) : IFantasyAnalysisService
    {
        private readonly Season _season = season.CurrentValue;
        private readonly FantasyScoring _scoring = scoring.CurrentValue;

        public async Task<List<BoomBust>> GetBoomBusts(Position position)
        {
            var weeklyFantasy = await fantasyDataService.GetWeeklyFantasy(position);
            var playersByPosition = await playersService.GetPlayersByPosition(position, true);
            return weeklyFantasy.Join(playersByPosition, wf => wf.PlayerId, p => p.PlayerId, 
                                       (wf, p) => new { WeeklyFantasy = wf, Player = p })
                                                       .GroupBy(wfp => wfp.Player, 
                                                                wfp => wfp.WeeklyFantasy.FantasyPoints, 
                                                                (key, f) => new {Player = key, FantasyPoints = f.ToList()})
                                                       .Select(wf => new { 
                                                                            wf.Player, 
                                                                            BoomCount = (double)wf.FantasyPoints.Count(fp => fp > settingsService.GetBoomSetting(position)), 
                                                                            BustCount = (double)wf.FantasyPoints.Count(fp => fp < settingsService.GetBustSetting(position)), 
                                                                            WeekCount = (double)wf.FantasyPoints.Count})                                                                        
                                                       .Select(bb => new BoomBust
                                                                {
                                                                     Season = _season.CurrentSeason,
                                                                     Player = bb.Player,
                                                                     BoomPercentage = bb.WeekCount > 0 ? Math.Round((bb.BoomCount/bb.WeekCount)*100) : 0,
                                                                     BustPercentage  = bb.WeekCount > 0 ? Math.Round(bb.BustCount/bb.WeekCount*100) : 0
                                                                }).ToList();
        }

        public async Task<List<FantasyPerformance>> GetFantasyPerformances(int teamId)
        {
            var teamMap = await playersService.GetTeam(teamId);
            var players = await playersService.GetPlayersByTeam(teamMap.Team);
            List<FantasyPerformance> teamFantasyPerformances = [];
            if (players.Count > 0)
            {
                foreach (var p in players)
                {
                    var player = await playersService.GetPlayer(p.PlayerId);
                    var fp = await GetFantasyPerformance(player);
                    if (fp != null) teamFantasyPerformances.Add(fp);
                }
            }
            return teamFantasyPerformances;
        }
        public async Task<List<FantasyPerformance>> GetFantasyPerformances(Position position)
        {
            List<FantasyPerformance> fantasyPerformances = [];
            var players = await playersService.GetPlayersByPosition(position, true);
            foreach (var player in players)
            {
                var fantasyPerformance = await GetFantasyPerformance(player);
                if (fantasyPerformance != null)
                   fantasyPerformances.Add(fantasyPerformance);
            }
            return fantasyPerformances.Where(f => f.AvgFantasy > 0).OrderByDescending(f => f.AvgFantasy).ToList();
        }

        public async Task<FantasyPerformance?> GetFantasyPerformance(Player player)
        {
            var weeklyFantasy = (await fantasyDataService.GetWeeklyFantasy(player.PlayerId)).Where(f => f.Season == _season.CurrentSeason);
            if (weeklyFantasy.Any())
            {
                var variance = CalculateVariance(weeklyFantasy.Select(w => w.FantasyPoints));
                return new FantasyPerformance
                {
                    PlayerId = player.PlayerId,
                    Name = player.Name,
                    Position = player.Position,
                    Season = _season.CurrentSeason,
                    TotalFantasy = weeklyFantasy.Sum(w => w.FantasyPoints),
                    AvgFantasy = weeklyFantasy.Average(w => w.FantasyPoints),
                    MinFantasy = weeklyFantasy.Min(w => w.FantasyPoints),
                    MaxFantasy = weeklyFantasy.Max(w => w.FantasyPoints),
                    Variance = variance,
                    StdDev = Math.Sqrt(variance)
                };
            }
            else return null;
        }
        public async Task<List<BoomBustByWeek>> GetBoomBustsByWeek(int playerId)
        {
            var player = await playersService.GetPlayer(playerId);
            var weeklyFantasy = await fantasyDataService.GetWeeklyFantasy(player.PlayerId);
            if (Enum.TryParse(player.Position, out Position position))
            {
                 return weeklyFantasy.Select(wf => new BoomBustByWeek
                 {
                    Season = _season.CurrentSeason,
                    Player = player,
                    Week = wf.Week,
                    Boom = wf.FantasyPoints > settingsService.GetBoomSetting(position),
                    Bust = wf.FantasyPoints < settingsService.GetBustSetting(position),
                    FantasyPoints = wf.FantasyPoints
                 }).OrderBy(b => b.Week).ToList();
            }
            else return Enumerable.Empty<BoomBustByWeek>().ToList();
        }

        public async Task<List<FantasyPercentage>> GetFantasyPercentages(Position position)
        {
            var players = await playersService.GetPlayersByPosition(position, true);
            List<FantasyPercentage> fantasyPercentages = [];
            if (position == Position.QB)
            {
                foreach (var player in players)
                {
                    var stats = await statsService.GetWeeklyData<WeeklyDataQB>(position, player.PlayerId);
                    var fantasy = await fantasyDataService.GetWeeklyFantasy(player.PlayerId);
                    var totalFantasy = fantasy.Sum(f => f.FantasyPoints)
                                     + stats.Sum(s => s.Int) * _scoring.PointsPerInterception
                                     + stats.Sum(s => s.Fumbles) * _scoring.PointsPerFumble;
                    if (stats.Count > 0 && totalFantasy > 0)
                    {
                        fantasyPercentages.Add(new FantasyPercentage
                        {
                            PlayerId = player.PlayerId,
                            Name = player.Name,
                            Position = player.Position,
                            PassYDShare = Math.Round((stats.Sum(s => s.Yards) * _scoring.PointsPerPassingYard) /totalFantasy , 4),
                            PassTDShare = Math.Round((stats.Sum(s => s.TD) * _scoring.PointsPerPassingTouchdown) /totalFantasy, 4),
                            RushYDShare = Math.Round(stats.Sum(s => s.RushingYards) * _scoring.PointsPerYard/totalFantasy , 4),
                            RushTDShare = Math.Round(stats.Sum(s => s.RushingTD) * _scoring.PointsPerTouchdown/totalFantasy, 4)
                        });
                    }
                }
            }
            else if (position == Position.RB)
            {
                foreach (var player in players)
                {
                    var stats = await statsService.GetWeeklyData<WeeklyDataRB>(position, player.PlayerId);
                    var fantasy = await fantasyDataService.GetWeeklyFantasy(player.PlayerId);
                    var totalFantasy = fantasy.Sum(f => f.FantasyPoints) + stats.Sum(s => s.Fumbles)* _scoring.PointsPerFumble;
                    if (stats.Count > 0 && totalFantasy > 0)
                    {
                        fantasyPercentages.Add(new FantasyPercentage
                        {
                            PlayerId = player.PlayerId,
                            Name = player.Name,
                            Position = player.Position,
                            RushYDShare = Math.Round(stats.Sum(s => s.RushingYds) * _scoring.PointsPerYard / totalFantasy, 4),
                            RushTDShare = Math.Round(stats.Sum(s => s.RushingTD) * _scoring.PointsPerTouchdown / totalFantasy, 4),
                            RecShare = Math.Round(stats.Sum(s => s.Receptions) * _scoring.PointsPerReception / totalFantasy, 4),
                            RecYDShare = Math.Round(stats.Sum(s => s.Yards) * _scoring.PointsPerYard / totalFantasy, 4),
                            RecTDShare = Math.Round(stats.Sum(s => s.ReceivingTD) * _scoring.PointsPerTouchdown / totalFantasy, 4)
                        });
                    }
                }
            }
            else if (position == Position.WR)
            {
                foreach (var player in players)
                {
                    var stats = await statsService.GetWeeklyData<WeeklyDataWR>(position, player.PlayerId);
                    var fantasy = await fantasyDataService.GetWeeklyFantasy(player.PlayerId);
                    var totalFantasy = fantasy.Sum(f => f.FantasyPoints) + stats.Sum(s => s.Fumbles) * _scoring.PointsPerFumble;
                    if (stats.Count > 0 && totalFantasy > 0)
                    {
                        fantasyPercentages.Add(new FantasyPercentage
                        {
                            PlayerId = player.PlayerId,
                            Name = player.Name,
                            Position = player.Position,
                            RushYDShare = Math.Round(stats.Sum(s => s.RushingYds) * _scoring.PointsPerYard / totalFantasy, 4),
                            RushTDShare = Math.Round(stats.Sum(s => s.RushingTD) * _scoring.PointsPerTouchdown / totalFantasy, 4),
                            RecShare = Math.Round(stats.Sum(s => s.Receptions) * _scoring.PointsPerReception / totalFantasy, 4),
                            RecYDShare = Math.Round(stats.Sum(s => s.Yards) * _scoring.PointsPerYard / totalFantasy, 4),
                            RecTDShare = Math.Round(stats.Sum(s => s.TD) * _scoring.PointsPerTouchdown / totalFantasy, 4)
                        });
                    }
                }
            }
            else if (position == Position.TE)
            {
                foreach (var player in players)
                {
                    var stats = await statsService.GetWeeklyData<WeeklyDataTE>(position, player.PlayerId);
                    var fantasy = await fantasyDataService.GetWeeklyFantasy(player.PlayerId);
                    var totalFantasy = fantasy.Sum(f => f.FantasyPoints) + stats.Sum(s => s.Fumbles) * _scoring.PointsPerFumble;
                    if (stats.Count > 0 && totalFantasy > 0)
                    {
                        fantasyPercentages.Add(new FantasyPercentage
                        {
                            PlayerId = player.PlayerId,
                            Name = player.Name,
                            Position = player.Position,
                            RushYDShare = Math.Round(stats.Sum(s => s.RushingYds) * _scoring.PointsPerYard / totalFantasy, 4),
                            RushTDShare = Math.Round(stats.Sum(s => s.RushingTD) * _scoring.PointsPerTouchdown / totalFantasy, 4),
                            RecShare = Math.Round(stats.Sum(s => s.Receptions) * _scoring.PointsPerReception / totalFantasy, 4),
                            RecYDShare = Math.Round(stats.Sum(s => s.Yards) * _scoring.PointsPerYard / totalFantasy, 4),
                            RecTDShare = Math.Round(stats.Sum(s => s.TD) * _scoring.PointsPerTouchdown / totalFantasy, 4)
                        });
                    }
                }
            }
            return CleanPercentageAmounts(fantasyPercentages);
        }

        public async Task<FantasyPercentage> GetQBSeasonFantasyPercentageByPlayerId(int season, int playerId)
        {
            var seasonData = (await statsService.GetSeasonData<SeasonDataQB>(Position.QB, playerId, true)).FirstOrDefault(s => s.Season == season);
            if (seasonData != null) 
            {
                var seasonFantasy = (await fantasyDataService.GetSeasonFantasy(playerId)).First(s => s.Season == season);
                var totalFantasy = seasonFantasy.FantasyPoints + seasonData.Int * _scoring.PointsPerInterception + seasonData.Fumbles * _scoring.PointsPerFumble;
                return new FantasyPercentage
                {
                    PlayerId =  playerId,
                    Season = season,
                    Position = Position.QB.ToString(),
                    PassYDShare = Math.Round(seasonData.Yards * _scoring.PointsPerPassingYard / totalFantasy, 4),
                    PassTDShare = Math.Round(seasonData.TD * _scoring.PointsPerPassingTouchdown / totalFantasy, 4),
                    RushYDShare = Math.Round(seasonData.RushingYards * _scoring.PointsPerYard / totalFantasy, 4),
                    RushTDShare = Math.Round(seasonData.RushingTD * _scoring.PointsPerTouchdown / totalFantasy, 4)
                };
            }
            return new();
        }

        public async Task<FantasyPercentage> GetRBSeasonFantasyPercentageByPlayerId(int season, int playerId)
        {
            var seasonData = (await statsService.GetSeasonData<SeasonDataRB>(Position.RB, playerId, true)).FirstOrDefault(s => s.Season == season);
            if (seasonData != null)
            {
                var seasonFantasy = (await fantasyDataService.GetSeasonFantasy(playerId)).First(s => s.Season == season);
                var totalFantasy = seasonFantasy.FantasyPoints + seasonData.Fumbles * _scoring.PointsPerFumble;
                return new FantasyPercentage
                {
                    PlayerId = playerId,
                    Season = season,
                    Position = Position.RB.ToString(),
                    RushYDShare = totalFantasy > 0 ? Math.Round(seasonData.RushingYds * _scoring.PointsPerYard / totalFantasy, 4) : 0,
                    RushTDShare = totalFantasy > 0 ? Math.Round(seasonData.RushingTD * _scoring.PointsPerTouchdown / totalFantasy, 4) : 0,
                    RecShare = totalFantasy > 0 ? Math.Round(seasonData.Receptions * _scoring.PointsPerReception / totalFantasy, 4) : 0,
                    RecYDShare = totalFantasy > 0 ? Math.Round(seasonData.Yards * _scoring.PointsPerYard / totalFantasy, 4) : 0,
                    RecTDShare = totalFantasy > 0 ? Math.Round(seasonData.ReceivingTD * _scoring.PointsPerTouchdown / totalFantasy, 4) : 0
                };
            }
            return new();
        }


        private static List<FantasyPercentage> CleanPercentageAmounts(List<FantasyPercentage> fantasyPercentages)
        {
            foreach (var fp in fantasyPercentages)
            {
                fp.PassYDShare = fp.PassYDShare > 1 ? 1 : fp.PassYDShare < 0 ? 0 : fp.PassYDShare;
                fp.PassTDShare = fp.PassTDShare > 1 ? 1 : fp.PassTDShare < 0 ? 0 : fp.PassTDShare;
                fp.RushYDShare = fp.RushYDShare > 1 ? 1 : fp.RushYDShare < 0 ? 0 : fp.RushYDShare;
                fp.RushTDShare = fp.RushTDShare > 1 ? 1 : fp.RushTDShare < 0 ? 0 : fp.RushTDShare;
                fp.RecShare = fp.RecShare > 1 ? 1 : fp.RecShare < 0 ? 0 : fp.RecShare;
                fp.RecYDShare = fp.RecYDShare > 1 ? 1 : fp.RecYDShare < 0 ? 0 : fp.RecYDShare;
                fp.RecTDShare = fp.RecTDShare > 1 ? 1 : fp.RecTDShare < 0 ? 0 : fp.RecTDShare;
            }

            return fantasyPercentages;
        }

        private static double CalculateVariance(IEnumerable<double> fantasyPoints)
        {
            var mean = fantasyPoints.Average();
            var count = fantasyPoints.Count();
            var sumOfSquares = 0.0;
            foreach (var fp in fantasyPoints)
                sumOfSquares += Math.Pow(fp - mean, 2);
            return sumOfSquares / count;
        }


    }
}
