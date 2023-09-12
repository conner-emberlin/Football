using Football.Projections.Interfaces;
using Football.Data.Models;
using Football.Models;
using Microsoft.Extensions.Options;
using Serilog;
using Football.Fantasy.Models;

namespace Football.Projections.Services
{
    public class StatProjectionCalculator : IStatProjectionCalculator
    {
        private readonly Tunings _tunings;
        private readonly WeeklyTunings _weeklyTunings;
        private readonly Season _season;
        private readonly ILogger _logger;
        public StatProjectionCalculator(IOptionsMonitor<Tunings> tunings, IOptionsMonitor<WeeklyTunings> weeklyTunings, IOptionsMonitor<Season> season, ILogger logger)
        {
            _tunings = tunings.CurrentValue;
            _weeklyTunings = weeklyTunings.CurrentValue;
            _season = season.CurrentValue;
            _logger = logger;
        }
        public SeasonDataQB CalculateStatProjection(List<SeasonDataQB> seasons)
        {
            _logger.Information("Calculating QB Stat Projections for {p}: {n}", seasons.First().PlayerId, seasons.First().Name);
            try
            {
                var recentWeight = seasons.Count > 1 ? _tunings.Weight : _tunings.SecondYearQBLeap;
                var recentSeason = seasons.Max(s => s.Season);
                var previousSeasons = seasons.Count - 1;
                var previousWeight = seasons.Count > 1 ? ((1 - _tunings.Weight) * ((double)1 / previousSeasons)) : 0;

                double avgComp = 0;
                double avgAtt = 0;
                double avgYd = 0;
                double avgTD = 0;
                double avgInt = 0;
                double avgSacks = 0;
                double avgRAtt = 0;
                double avgRYd = 0;
                double avgRTD = 0;
                double avgFum = 0;

                foreach(var s in seasons)
                {
                    if(s.Games < _season.Games)
                    {
                        s.Completions += (s.Completions / s.Games) * (_season.Games - s.Games);
                        s.Attempts += (s.Attempts / s.Games) * (_season.Games - s.Games);
                        s.Yards += (s.Yards / s.Games) * (_season.Games - s.Games);
                        s.TD += (s.TD / s.Games) * (_season.Games - s.Games);
                        s.Int += (s.Int / s.Games) * (_season.Games - s.Games);
                        s.Sacks += (s.Sacks / s.Games) * (_season.Games - s.Games);
                        s.RushingAttempts += (s.RushingAttempts / s.Games) * (_season.Games - s.Games);
                        s.RushingYards += (s.RushingYards / s.Games) * (_season.Games - s.Games);
                        s.RushingTD += (s.RushingTD / s.Games) * (_season.Games - s.Games);
                        s.Fumbles += (s.Fumbles / s.Games) * (_season.Games - s.Games);
                    }

                    avgComp += s.Season == recentSeason ? recentWeight * s.Completions : previousWeight * s.Completions;
                    avgAtt += s.Season == recentSeason ? recentWeight * s.Attempts : previousWeight * s.Attempts;
                    avgYd += s.Season == recentSeason ? recentWeight * s.Yards : previousWeight * s.Yards;
                    avgTD += s.Season == recentSeason ? recentWeight * s.TD : previousWeight * s.TD;
                    avgInt += s.Season == recentSeason ? recentWeight * s.Int : previousWeight * s.Int;
                    avgSacks += s.Season == recentSeason ? recentWeight * s.Sacks : previousWeight * s.Sacks;
                    avgRAtt += s.Season == recentSeason ? recentWeight * s.RushingAttempts : previousWeight * s.RushingAttempts;
                    avgRYd += s.Season == recentSeason ? recentWeight * s.RushingYards : previousWeight * s.RushingYards;              
                    avgRTD += s.Season == recentSeason ? recentWeight * s.RushingTD : previousWeight * s.RushingTD;
                    avgFum += s.Season == recentSeason ? recentWeight * s.Fumbles : previousWeight * s.Fumbles;
                }
                return new SeasonDataQB
                {
                    PlayerId = seasons.First().PlayerId,
                    Name = seasons.First().Name,
                    Season = _season.CurrentSeason,
                    Games = _season.Games,
                    Completions = avgComp,
                    Attempts = avgAtt,
                    Yards = avgYd,
                    TD = avgTD,
                    Int = avgInt,
                    Sacks = avgSacks,
                    RushingAttempts = avgRAtt,
                    RushingYards = avgRYd,
                    RushingTD = avgRTD,
                    Fumbles = avgFum
                };

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }

        public SeasonDataRB CalculateStatProjection(List<SeasonDataRB> seasons)
        {
            _logger.Information("Calculating RB Stat Projections for {p}: {n}", seasons.First().PlayerId, seasons.First().Name);
            try
            {
                var recentWeight = seasons.Count > 1 ? _tunings.Weight : _tunings.SecondYearRBLeap;
                var recentSeason = seasons.Max(s => s.Season);
                var previousSeasons = seasons.Count - 1;
                var previousWeight = seasons.Count > 1 ? ((1 - _tunings.Weight) * ((double)1 / previousSeasons)) : 0;

                double avgRAtt = 0;
                double avgRYd = 0;
                double avgRTD = 0;
                double avgRec = 0;
                double avgTgt = 0;
                double avgYds = 0;
                double avgRecTD = 0;
                double avgFum = 0;

                foreach (var s in seasons)
                {
                    if (s.Games < _season.Games)
                    {
                        s.RushingAtt += (s.RushingAtt / s.Games) * (_season.Games - s.Games);
                        s.RushingYds += (s.RushingYds / s.Games) * (_season.Games - s.Games);
                        s.RushingTD += (s.RushingTD / s.Games) * (_season.Games - s.Games);
                        s.Receptions += (s.Receptions/ s.Games) * (_season.Games - s.Games);
                        s.Targets += (s.Targets / s.Games) * (_season.Games - s.Games);
                        s.Yards += (s.Yards / s.Games) * (_season.Games - s.Games);
                        s.ReceivingTD += (s.ReceivingTD / s.Games) * (_season.Games - s.Games);
                        s.Fumbles += (s.Fumbles / s.Games) * (_season.Games - s.Games);
                    }
                    avgRAtt += s.Season == recentSeason ? recentWeight * s.RushingAtt : previousWeight * s.RushingAtt;
                    avgRYd += s.Season == recentSeason ? recentWeight * s.RushingYds : previousWeight * s.RushingYds;
                    avgRTD += s.Season == recentSeason ? recentWeight * s.RushingTD : previousWeight * s.RushingTD;
                    avgFum += s.Season == recentSeason ? recentWeight * s.Fumbles : previousWeight * s.Fumbles;
                    avgRec += s.Season == recentSeason ? recentWeight * s.Receptions : previousWeight * s.Receptions;
                    avgTgt += s.Season == recentSeason ? recentWeight * s.Targets : previousWeight * s.Targets;
                    avgYds += s.Season == recentSeason ? recentWeight * s.Yards : previousWeight * s.Yards;
                    avgRecTD += s.Season == recentSeason ? recentWeight * s.ReceivingTD : previousWeight * s.ReceivingTD;
                }
                return new SeasonDataRB
                {
                    PlayerId = seasons.First().PlayerId,
                    Name = seasons.First().Name,
                    Season = _season.CurrentSeason,
                    Games = _season.Games,
                    RushingAtt = avgRAtt,
                    RushingYds = avgRYd,
                    RushingTD = avgRTD,
                    Receptions = avgRec,
                    Targets = avgTgt,
                    Yards = avgYds,
                    ReceivingTD = avgRecTD,
                    Fumbles = avgFum
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }
        public SeasonDataWR CalculateStatProjection(List<SeasonDataWR> seasons)
        {
            _logger.Information("Calculating WR Stat Projections for {p}: {n}", seasons.First().PlayerId, seasons.First().Name);
            try
            {
                var recentWeight = seasons.Count > 1 ? _tunings.Weight : _tunings.SecondYearWRLeap;
                var recentSeason = seasons.Max(s => s.Season);
                var previousSeasons = seasons.Count - 1;
                var previousWeight = seasons.Count > 1 ? ((1 - _tunings.Weight) * ((double)1 / previousSeasons)) : 0;

                double avgRec = 0;
                double avgTgt = 0;
                double avgYds = 0;
                double avgTD = 0;
                double avgRAtt = 0;
                double avgRYd = 0;
                double avgRTD = 0;
                double avgFum = 0;

                foreach (var s in seasons)
                {
                    if (s.Games < _season.Games)
                    {
                        s.Receptions += (s.Receptions / s.Games) * (_season.Games - s.Games);
                        s.Targets += (s.Targets / s.Games) * (_season.Games - s.Games);
                        s.Yards += (s.Yards / s.Games) * (_season.Games - s.Games);
                        s.TD += (s.TD / s.Games) * (_season.Games - s.Games);
                        s.RushingAtt += (s.RushingAtt / s.Games) * (_season.Games - s.Games);
                        s.RushingYds += (s.RushingYds / s.Games) * (_season.Games - s.Games);
                        s.RushingTD += (s.RushingTD / s.Games) * (_season.Games - s.Games);
                        s.Fumbles += (s.Fumbles / s.Games) * (_season.Games - s.Games);
                    }
                    avgRAtt += s.Season == recentSeason ? recentWeight * s.RushingAtt : previousWeight * s.RushingAtt;
                    avgRYd += s.Season == recentSeason ? recentWeight * s.RushingYds : previousWeight * s.RushingYds;
                    avgRTD += s.Season == recentSeason ? recentWeight * s.RushingTD : previousWeight * s.RushingTD;
                    avgFum += s.Season == recentSeason ? recentWeight * s.Fumbles : previousWeight * s.Fumbles;
                    avgRec += s.Season == recentSeason ? recentWeight * s.Receptions : previousWeight * s.Receptions;
                    avgTgt += s.Season == recentSeason ? recentWeight * s.Targets : previousWeight * s.Targets;
                    avgYds += s.Season == recentSeason ? recentWeight * s.Yards : previousWeight * s.Yards;
                    avgTD += s.Season == recentSeason ? recentWeight * s.TD : previousWeight * s.TD;
                }
                return new SeasonDataWR
                {
                    PlayerId = seasons.First().PlayerId,
                    Name = seasons.First().Name,
                    Season = _season.CurrentSeason,
                    Games = _season.Games,
                    Receptions = avgRec,
                    Targets = avgTgt,
                    Yards = avgYds,
                    TD = avgTD,
                    Long = seasons.Average(s => s.Long),
                    RushingAtt = avgRAtt,
                    RushingYds = avgRYd,
                    RushingTD = avgRTD,
                    Fumbles = avgFum
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }
        public SeasonDataTE CalculateStatProjection(List<SeasonDataTE> seasons)
        {
            _logger.Information("Calculating TE Stat Projections for {p}: {n}", seasons.First().PlayerId, seasons.First().Name);
            try
            {
                var recentWeight = seasons.Count > 1 ? _tunings.Weight : _tunings.SecondYearWRLeap;
                var recentSeason = seasons.Max(s => s.Season);
                var previousSeasons = seasons.Count - 1;
                var previousWeight = seasons.Count > 1 ? ((1 - _tunings.Weight) * ((double)1 / previousSeasons)) : 0;

                double avgRec = 0;
                double avgTgt = 0;
                double avgYds = 0;
                double avgTD = 0;
                double avgRAtt = 0;
                double avgRYd = 0;
                double avgRTD = 0;
                double avgFum = 0;

                foreach (var s in seasons)
                {
                    if (s.Games < _season.Games)
                    {
                        s.Receptions += (s.Receptions / s.Games) * (_season.Games - s.Games);
                        s.Targets += (s.Targets / s.Games) * (_season.Games - s.Games);
                        s.Yards += (s.Yards / s.Games) * (_season.Games - s.Games);
                        s.TD += (s.TD / s.Games) * (_season.Games - s.Games);
                        s.RushingAtt += (s.RushingAtt / s.Games) * (_season.Games - s.Games);
                        s.RushingYds += (s.RushingYds / s.Games) * (_season.Games - s.Games);
                        s.RushingTD += (s.RushingTD / s.Games) * (_season.Games - s.Games);
                        s.Fumbles += (s.Fumbles / s.Games) * (_season.Games - s.Games);
                    }
                    avgRAtt += s.Season == recentSeason ? recentWeight * s.RushingAtt : previousWeight * s.RushingAtt;
                    avgRYd += s.Season == recentSeason ? recentWeight * s.RushingYds : previousWeight * s.RushingYds;
                    avgRTD += s.Season == recentSeason ? recentWeight * s.RushingTD : previousWeight * s.RushingTD;
                    avgFum += s.Season == recentSeason ? recentWeight * s.Fumbles : previousWeight * s.Fumbles;
                    avgRec += s.Season == recentSeason ? recentWeight * s.Receptions : previousWeight * s.Receptions;
                    avgTgt += s.Season == recentSeason ? recentWeight * s.Targets : previousWeight * s.Targets;
                    avgYds += s.Season == recentSeason ? recentWeight * s.Yards : previousWeight * s.Yards;
                    avgTD += s.Season == recentSeason ? recentWeight * s.TD : previousWeight * s.TD;
                }
                return new SeasonDataTE
                {
                    PlayerId = seasons.First().PlayerId,
                    Name = seasons.First().Name,
                    Season = _season.CurrentSeason,
                    Games = _season.Games,
                    Receptions = avgRec,
                    Targets = avgTgt,
                    Yards = avgYds,
                    TD = avgTD,
                    Long = seasons.Average(s => s.Long),
                    RushingAtt = avgRAtt,
                    RushingYds = avgRYd,
                    RushingTD = avgRTD,
                    Fumbles = avgFum
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }

        public SeasonFantasy CalculateStatProjection(List<SeasonFantasy> seasons)
        {
            _logger.Information("Calculating Fantasy Stat projections for {p}", seasons.First().PlayerId);
            try
            {
                var recentWeight = seasons.Count > 1 ? _tunings.Weight : 1;
                var recentSeason = seasons.Max(s => s.Season);
                var previousSeasons = seasons.Count - 1;
                var previousWeight = seasons.Count > 1 ? ((1 - _tunings.Weight) * ((double)1 / previousSeasons)) : 0;
                double avgFp = 0;
                foreach (var season in seasons)
                {
                    if (season.Games < _season.Games)
                    {
                        season.FantasyPoints += (season.FantasyPoints / season.Games) * (_season.Games - season.Games);
                    }
                    avgFp += season.Season == recentSeason ? recentWeight * season.FantasyPoints : previousWeight * season.FantasyPoints;
                }
                return new SeasonFantasy
                {
                    PlayerId = seasons.First().PlayerId,
                    Season = _season.CurrentSeason,
                    Games = _season.Games,
                    FantasyPoints = avgFp
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }
        public WeeklyFantasy CalculateWeeklyAverage(List<WeeklyFantasy> weeks)
        {
            _logger.Information("Calculating Weekly fantasy averages for player {playerId}", weeks.First().PlayerId);
            var recentWeight = weeks.Count > 1 ? _weeklyTunings.RecentWeekWeight : 1;
            var recentWeek = weeks.Max(w => w.Week);
            var previousWeeks = weeks.Count - 1;
            var previousWeight = weeks.Count > 1 ? ((1 - _weeklyTunings.RecentWeekWeight) * ((double)1 / previousWeeks)) : 0;

            double ftp = 0;
            foreach (var w in weeks)
            {
                ftp += w.Week == recentWeek ? recentWeight * w.FantasyPoints : previousWeight * w.FantasyPoints;
            }
            return new WeeklyFantasy
            {
                PlayerId = weeks.First().PlayerId,
                Season = _season.CurrentSeason,
                Week = recentWeek + 1,
                Games = 1,
                Name = weeks.First().Name,
                Position = weeks.First().Position,
                FantasyPoints = ftp
            };

        }
        public WeeklyDataQB CalculateWeeklyAverage(List<WeeklyDataQB> weeks)
        {
            _logger.Information("Calculating Weekly averages for player {playerId}", weeks.First().PlayerId);
            var recentWeight = weeks.Count > 1 ? _weeklyTunings.RecentWeekWeight : 1;
            var recentWeek = weeks.Max(w => w.Week);
            var previousWeeks = weeks.Count - 1;
            var previousWeight = weeks.Count > 1 ? ((1 - _weeklyTunings.RecentWeekWeight) * ((double)1 / previousWeeks)) : 0;

            double avgComp = 0;
            double avgAtt = 0;
            double avgYd = 0;
            double avgTD = 0;
            double avgInt = 0;
            double avgSacks = 0;
            double avgRAtt = 0;
            double avgRYd = 0;
            double avgRTD = 0;
            double avgFum = 0;

            foreach (var w in weeks)
            {
                avgComp += w.Week == recentWeek ? recentWeight * w.Completions : previousWeight * w.Completions;
                avgAtt += w.Week == recentWeek ? recentWeight * w.Attempts : previousWeight * w.Attempts;
                avgYd += w.Week == recentWeek ? recentWeight * w.Yards : previousWeight * w.Yards;
                avgTD += w.Week == recentWeek ? recentWeight * w.TD : previousWeight * w.TD;
                avgInt += w.Week == recentWeek ? recentWeight * w.Int : previousWeight * w.Int;
                avgSacks += w.Week == recentWeek ? recentWeight * w.Sacks : previousWeight * w.Sacks;
                avgRAtt += w.Week == recentWeek ? recentWeight * w.RushingAttempts : previousWeight * w.RushingAttempts;
                avgRYd += w.Week == recentWeek ? recentWeight * w.RushingYards : previousWeight * w.RushingYards;
                avgRTD += w.Week == recentWeek ? recentWeight * w.RushingTD : previousWeight * w.RushingTD;
                avgFum += w.Week == recentWeek ? recentWeight * w.Fumbles : previousWeight * w.Fumbles;
            }

            return new WeeklyDataQB
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = recentWeek + 1,
                Completions = avgComp,
                Attempts = avgAtt,
                Yards = avgYd,
                TD = avgTD,
                Int = avgInt,
                Sacks = avgSacks,
                RushingAttempts = avgRAtt,
                RushingYards = avgRYd,
                RushingTD = avgRTD,
                Fumbles = avgFum
            };
        }

        public WeeklyDataRB CalculateWeeklyAverage(List<WeeklyDataRB> weeks)
        {
            _logger.Information("Calculating Weekly averages for player {playerId}", weeks.First().PlayerId);
            var recentWeight = weeks.Count > 1 ? _weeklyTunings.RecentWeekWeight : 1;
            var recentWeek = weeks.Max(w => w.Week);
            var previousWeeks = weeks.Count - 1;
            var previousWeight = weeks.Count > 1 ? ((1 - _weeklyTunings.RecentWeekWeight) * ((double)1 / previousWeeks)) : 0;

            double avgRAtt = 0;
            double avgRYd = 0;
            double avgRTD = 0;
            double avgRec = 0;
            double avgTgt = 0;
            double avgYds = 0;
            double avgRecTD = 0;
            double avgFum = 0;

            foreach (var w in weeks)
            {
                avgRAtt += w.Week == recentWeek ? recentWeight * w.RushingAtt : previousWeight * w.RushingAtt;
                avgRYd += w.Week == recentWeek ? recentWeight * w.RushingYds : previousWeight * w.RushingYds;
                avgRTD += w.Week == recentWeek ? recentWeight * w.RushingTD : previousWeight * w.RushingTD;
                avgRec += w.Week == recentWeek ? recentWeight * w.Receptions : previousWeight * w.Receptions;
                avgTgt += w.Week == recentWeek ? recentWeight * w.Targets : previousWeight * w.Targets;
                avgYds += w.Week == recentWeek ? recentWeight * w.Yards : previousWeight * w.Yards;
                avgRecTD += w.Week == recentWeek ? recentWeight * w.ReceivingTD : previousWeight * w.ReceivingTD;
                avgFum += w.Week == recentWeek ? recentWeight * w.Fumbles : previousWeight * w.Fumbles;
            }

            return new WeeklyDataRB
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = recentWeek + 1,
                RushingAtt = avgRAtt,
                RushingYds = avgRYd,
                RushingTD = avgRTD,
                Receptions = avgRec,
                Targets = avgTgt,
                Yards = avgYds,
                ReceivingTD = avgRecTD,
                Fumbles = avgFum
            };
        }
        public WeeklyDataWR CalculateWeeklyAverage(List<WeeklyDataWR> weeks)
        {
            _logger.Information("Calculating Weekly averages for player {playerId}", weeks.First().PlayerId);
            var recentWeight = weeks.Count > 1 ? _weeklyTunings.RecentWeekWeight : 1;
            var recentWeek = weeks.Max(w => w.Week);
            var previousWeeks = weeks.Count - 1;
            var previousWeight = weeks.Count > 1 ? ((1 - _weeklyTunings.RecentWeekWeight) * ((double)1 / previousWeeks)) : 0;

            double avgRec = 0;
            double avgTgt = 0;
            double avgYds = 0;
            double avgTD = 0;
            double avgRAtt = 0;
            double avgRYd = 0;
            double avgRTD = 0;
            double avgFum = 0;

            foreach (var w in weeks)
            {
                avgRec += w.Week == recentWeek ? recentWeight * w.Receptions : previousWeight * w.Receptions;
                avgTgt += w.Week == recentWeek ? recentWeight * w.Targets : previousWeight * w.Targets;
                avgYds += w.Week == recentWeek ? recentWeight * w.Yards : previousWeight * w.Yards;
                avgTD += w.Week == recentWeek ? recentWeight * w.TD : previousWeight * w.TD;
                avgRAtt += w.Week == recentWeek ? recentWeight * w.RushingAtt : previousWeight * w.RushingAtt;
                avgRYd += w.Week == recentWeek ? recentWeight * w.RushingYds : previousWeight * w.RushingYds;
                avgRTD += w.Week == recentWeek ? recentWeight * w.RushingTD : previousWeight * w.RushingTD;
                avgFum += w.Week == recentWeek ? recentWeight * w.Fumbles : previousWeight * w.Fumbles;
            }
            return new WeeklyDataWR
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = recentWeek + 1,
                Receptions = avgRec,
                Targets = avgTgt,
                Yards = avgYds,
                TD = avgTD,
                RushingAtt = avgRAtt,
                RushingYds = avgRYd,
                RushingTD = avgRTD,
                Fumbles = avgFum
            };
        }
        public WeeklyDataTE CalculateWeeklyAverage(List<WeeklyDataTE> weeks)
        {
            _logger.Information("Calculating Weekly averages for player {playerId}", weeks.First().PlayerId);
            var recentWeight = weeks.Count > 1 ? _weeklyTunings.RecentWeekWeight : 1;
            var recentWeek = weeks.Max(w => w.Week);
            var previousWeeks = weeks.Count - 1;
            var previousWeight = weeks.Count > 1 ? ((1 - _weeklyTunings.RecentWeekWeight) * ((double)1 / previousWeeks)) : 0;

            double avgRec = 0;
            double avgTgt = 0;
            double avgYds = 0;
            double avgTD = 0;
            double avgRAtt = 0;
            double avgRYd = 0;
            double avgRTD = 0;
            double avgFum = 0;

            foreach (var w in weeks)
            {
                avgRec += w.Week == recentWeek ? recentWeight * w.Receptions : previousWeight * w.Receptions;
                avgTgt += w.Week == recentWeek ? recentWeight * w.Targets : previousWeight * w.Targets;
                avgYds += w.Week == recentWeek ? recentWeight * w.Yards : previousWeight * w.Yards;
                avgTD += w.Week == recentWeek ? recentWeight * w.TD : previousWeight * w.TD;
                avgRAtt += w.Week == recentWeek ? recentWeight * w.RushingAtt : previousWeight * w.RushingAtt;
                avgRYd += w.Week == recentWeek ? recentWeight * w.RushingYds : previousWeight * w.RushingYds;
                avgRTD += w.Week == recentWeek ? recentWeight * w.RushingTD : previousWeight * w.RushingTD;
                avgFum += w.Week == recentWeek ? recentWeight * w.Fumbles : previousWeight * w.Fumbles;
            }
            return new WeeklyDataTE
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = recentWeek + 1,
                Receptions = avgRec,
                Targets = avgTgt,
                Yards = avgYds,
                TD = avgTD,
                RushingAtt = avgRAtt,
                RushingYds = avgRYd,
                RushingTD = avgRTD,
                Fumbles = avgFum
            };
        }
    }
}
