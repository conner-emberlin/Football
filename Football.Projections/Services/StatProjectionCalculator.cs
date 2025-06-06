﻿using Football.Fantasy.Models;
using Football.Models;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Microsoft.Extensions.Options;

namespace Football.Projections.Services
{
    public class StatProjectionCalculator(IOptionsMonitor<Season> season) : IStatProjectionCalculator
    {
        private readonly Season _season = season.CurrentValue;

        public SeasonDataQB CalculateStatProjection(List<SeasonDataQB> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames)
        {
            var secondYearLeap = true;

            if (seasons.Count == 0) return new SeasonDataQB();

            if (seasons.Count > 1)
            {
                var recentSeasonData = seasons.OrderByDescending(s => s.Season).First();
                var backupSeasons = seasons.Where(s => s.Attempts > 0 && s.Attempts / s.Games <= 0.5 * recentSeasonData.Attempts / recentSeasonData.Games).Select(s => s.Season);
                secondYearLeap = !backupSeasons.Any();
                seasons = seasons.Where(s => !backupSeasons.Contains(s.Season)).ToList();
            }

            var recentWeight = seasons.Count > 1 ? tunings.QBWeight : secondYearLeap ? tunings.SecondYearQBLeap : 1;
            var recentSeason = seasons.Max(s => s.Season);
            var previousSeasons = seasons.Count - 1;
            var previousWeight = seasons.Count > 1 ? ((1 - tunings.QBWeight) * ((double)1 / previousSeasons)) : 0;

            var avgComp = 0.0;
            var avgAtt = 0.0;
            var avgYd = 0.0;
            var avgTD = 0.0;
            var avgInt = 0.0;
            var avgSacks = 0.0;
            var avgRAtt = 0.0;
            var avgRYd = 0.0;
            var avgRTD = 0.0;
            var avgFum = 0.0;

            foreach (var s in seasons)
            {
                if (s.Season == _season.CurrentSeason - 1) s.Games -= gamesPlayedInjured;

                if (s.Games < seasonGames)
                {
                    s.Completions += (s.Completions / s.Games) * (seasonGames - s.Games);
                    s.Attempts += (s.Attempts / s.Games) * (seasonGames - s.Games);
                    s.Yards += (s.Yards / s.Games) * (seasonGames - s.Games);
                    s.TD += (s.TD / s.Games) * (seasonGames - s.Games);
                    s.Int += (s.Int / s.Games) * (seasonGames - s.Games);
                    s.Sacks += (s.Sacks / s.Games) * (seasonGames - s.Games);
                    s.RushingAttempts += (s.RushingAttempts / s.Games) * (seasonGames - s.Games);
                    s.RushingYards += (s.RushingYards / s.Games) * (seasonGames - s.Games);
                    s.RushingTD += (s.RushingTD / s.Games) * (seasonGames - s.Games);
                    s.Fumbles += (s.Fumbles / s.Games) * (seasonGames - s.Games);
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
                Games = seasonGames,
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

        public SeasonDataRB CalculateStatProjection(List<SeasonDataRB> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames)
        {
            var secondYearLeap = true;

            if (seasons.Count == 0) return new SeasonDataRB();

            if (seasons.Count > 1)
            {
                var recentSeasonData = seasons.OrderByDescending(s => s.Season).First();
                var backupSeasons = seasons.Where(s => (s.RushingAtt > 0 && s.RushingAtt / s.Games <= 0.5 * recentSeasonData.RushingAtt / recentSeasonData.Games)
                                              || (s.Targets > 0 && s.Targets / s.Games <= 0.5 * recentSeasonData.Targets / recentSeasonData.Games)).Select(s => s.Season);
                secondYearLeap = !backupSeasons.Any();
                seasons = seasons.Where(s => !backupSeasons.Contains(s.Season)).ToList();
            }

            var recentWeight = seasons.Count > 1 ? tunings.Weight : secondYearLeap ? tunings.SecondYearRBLeap : 1;
            var recentSeason = seasons.Max(s => s.Season);
            var previousSeasons = seasons.Count - 1;
            var previousWeight = seasons.Count > 1 ? ((1 - tunings.Weight) * ((double)1 / previousSeasons)) : 0;

            var avgRAtt = 0.0;
            var avgRYd = 0.0;
            var avgRTD = 0.0;
            var avgRec = 0.0;
            var avgTgt = 0.0;
            var avgYds = 0.0;
            var avgRecTD = 0.0;
            var avgFum = 0.0;

            foreach (var s in seasons)
            {
                if (s.Season == _season.CurrentSeason - 1) s.Games -= gamesPlayedInjured;

                if (s.Games < seasonGames)
                {
                    s.RushingAtt += (s.RushingAtt / s.Games) * (seasonGames - s.Games);
                    s.RushingYds += (s.RushingYds / s.Games) * (seasonGames - s.Games);
                    s.RushingTD += (s.RushingTD / s.Games) * (seasonGames - s.Games);
                    s.Receptions += (s.Receptions / s.Games) * (seasonGames - s.Games);
                    s.Targets += (s.Targets / s.Games) * (seasonGames - s.Games);
                    s.Yards += (s.Yards / s.Games) * (seasonGames - s.Games);
                    s.ReceivingTD += (s.ReceivingTD / s.Games) * (seasonGames - s.Games);
                    s.Fumbles += (s.Fumbles / s.Games) * (seasonGames - s.Games);
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
                Games = seasonGames,
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
        public SeasonDataWR CalculateStatProjection(List<SeasonDataWR> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames)
        {
            if (seasons.Count == 0) return new SeasonDataWR();

            if (seasons.Count > 3) seasons = seasons.Where(s => s.Season != seasons.Min(s => s.Season)).ToList();

            var recentWeight = seasons.Count > 1 ? tunings.Weight : tunings.SecondYearWRLeap;
            var recentSeason = seasons.Max(s => s.Season);
            var previousSeasons = seasons.Count - 1;
            var previousWeight = seasons.Count > 1 ? ((1 - tunings.Weight) * ((double)1 / previousSeasons)) : 0;

            var avgRec = 0.0;
            var avgTgt = 0.0;
            var avgYds = 0.0;
            var avgTD = 0.0;
            var avgRAtt = 0.0;
            var avgRYd = 0.0;
            var avgRTD = 0.0;
            var avgFum = 0.0;

            foreach (var s in seasons)
            {
                if (s.Season == _season.CurrentSeason - 1) s.Games -= gamesPlayedInjured;

                if (s.Games < seasonGames)
                {
                    s.Receptions += (s.Receptions / s.Games) * (seasonGames - s.Games);
                    s.Targets += (s.Targets / s.Games) * (seasonGames - s.Games);
                    s.Yards += (s.Yards / s.Games) * (seasonGames - s.Games);
                    s.TD += (s.TD / s.Games) * (seasonGames - s.Games);
                    s.RushingAtt += (s.RushingAtt / s.Games) * (seasonGames - s.Games);
                    s.RushingYds += (s.RushingYds / s.Games) * (seasonGames - s.Games);
                    s.RushingTD += (s.RushingTD / s.Games) * (seasonGames - s.Games);
                    s.Fumbles += (s.Fumbles / s.Games) * (seasonGames - s.Games);
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
                Games = seasonGames,
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
        public SeasonDataTE CalculateStatProjection(List<SeasonDataTE> seasons, double gamesPlayedInjured, Tunings tunings, int seasonGames)
        {
            var secondYearLeap = true;

            if (seasons.Count == 0) return new SeasonDataTE();

            if (seasons.Count > 1)
            {
                var recentSeasonData = seasons.OrderByDescending(s => s.Season).First();
                var backupSeasons = seasons.Where(s => s.Targets > 0 && s.Targets / s.Games <= 0.5 * recentSeasonData.Targets / recentSeasonData.Games).Select(s => s.Season);
                secondYearLeap = !backupSeasons.Any();
                seasons = seasons.Where(s => !backupSeasons.Contains(s.Season)).ToList();
            }
            
            var recentWeight = seasons.Count > 1 ? tunings.Weight : secondYearLeap ? tunings.SecondYearTELeap : 1;
            var recentSeason = seasons.Max(s => s.Season);
            var previousSeasons = seasons.Count - 1;
            var previousWeight = seasons.Count > 1 ? ((1 - tunings.Weight) * ((double)1 / previousSeasons)) : 0;

            var avgRec = 0.0;
            var avgTgt = 0.0;
            var avgYds = 0.0;
            var avgTD = 0.0;
            var avgRAtt = 0.0;
            var avgRYd = 0.0;
            var avgRTD = 0.0;
            var avgFum = 0.0;

            foreach (var s in seasons)
            {
                if (s.Season == _season.CurrentSeason - 1) s.Games -= gamesPlayedInjured;

                if (s.Games < seasonGames)
                {
                    s.Receptions += (s.Receptions / s.Games) * (seasonGames - s.Games);
                    s.Targets += (s.Targets / s.Games) * (seasonGames - s.Games);
                    s.Yards += (s.Yards / s.Games) * (seasonGames - s.Games);
                    s.TD += (s.TD / s.Games) * (seasonGames - s.Games);
                    s.RushingAtt += (s.RushingAtt / s.Games) * (seasonGames - s.Games);
                    s.RushingYds += (s.RushingYds / s.Games) * (seasonGames - s.Games);
                    s.RushingTD += (s.RushingTD / s.Games) * (seasonGames - s.Games);
                    s.Fumbles += (s.Fumbles / s.Games) * (seasonGames - s.Games);
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
                Games = seasonGames,
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
       
        public WeeklyDataQB WeightedWeeklyAverage(List<WeeklyDataQB> weeks, int currentWeek, WeeklyTunings weeklyTunings)
        {
            if (weeks.Count == 0) return new WeeklyDataQB();

            if (weeks.Count < weeklyTunings.MinWeekWeighted) return CalculateWeeklyAverage(weeks, currentWeek);

            var recentWeeks = weeks.Select(w => w.Week).OrderByDescending(w => w).Take(weeklyTunings.RecentWeeks);
            var recentWeeksWeight = weeklyTunings.RecentWeekWeight / recentWeeks.Count();
            var olderWeekCount = weeks.Count(w => !recentWeeks.Contains(w.Week));
            var olderWeekWeight = (1 - weeklyTunings.RecentWeekWeight) / olderWeekCount;
            WeeklyDataQB weightedAverage = new()
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
            };

            foreach (var w in weeks)
            {
                weightedAverage.Completions += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Completions : olderWeekWeight * w.Completions;
                weightedAverage.Attempts += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Attempts : olderWeekWeight * w.Attempts;
                weightedAverage.Yards += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Yards : olderWeekWeight * w.Yards;
                weightedAverage.TD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.TD : olderWeekWeight * w.TD;
                weightedAverage.Int += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Int : olderWeekWeight * w.Int;
                weightedAverage.Sacks += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Sacks : olderWeekWeight * w.Sacks;
                weightedAverage.RushingAttempts += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingAttempts : olderWeekWeight * w.RushingAttempts;
                weightedAverage.RushingYards += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingYards : olderWeekWeight * w.RushingYards;
                weightedAverage.RushingTD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingTD : olderWeekWeight * w.RushingTD;
                weightedAverage.Fumbles += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Fumbles : olderWeekWeight * w.Fumbles;
            }
            return weightedAverage;
        }

        public WeeklyDataRB WeightedWeeklyAverage(List<WeeklyDataRB> weeks, int currentWeek, WeeklyTunings weeklyTunings)
        {
            if (weeks.Count == 0) return new WeeklyDataRB();

            if (weeks.Count < weeklyTunings.MinWeekWeighted) return CalculateWeeklyAverage(weeks, currentWeek);

            var recentWeeks = weeks.Select(w => w.Week).OrderByDescending(w => w).Take(weeklyTunings.RecentWeeks);
            var recentWeeksWeight = weeklyTunings.RecentWeekWeight / recentWeeks.Count();
            var olderWeekCount = weeks.Count(w => !recentWeeks.Contains(w.Week));
            var olderWeekWeight = (1 - weeklyTunings.RecentWeekWeight) / olderWeekCount;
            WeeklyDataRB weightedAverage = new()
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
            };

            foreach (var w in weeks)
            {
                weightedAverage.RushingAtt += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingAtt : olderWeekWeight * w.RushingAtt;
                weightedAverage.RushingYds += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingYds : olderWeekWeight * w.RushingYds;
                weightedAverage.RushingTD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingTD : olderWeekWeight * w.RushingTD;
                weightedAverage.Receptions += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Receptions : olderWeekWeight * w.Receptions;
                weightedAverage.Targets += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Targets : olderWeekWeight * w.Targets;
                weightedAverage.Yards += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Yards : olderWeekWeight * w.Yards;
                weightedAverage.ReceivingTD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.ReceivingTD : olderWeekWeight * w.ReceivingTD;
                weightedAverage.Fumbles += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Fumbles : olderWeekWeight * w.Fumbles;
            }
            return weightedAverage;
        }

        public WeeklyDataWR WeightedWeeklyAverage(List<WeeklyDataWR> weeks, int currentWeek, WeeklyTunings weeklyTunings)
        {
            if (weeks.Count == 0) return new WeeklyDataWR();

            if (weeks.Count < weeklyTunings.MinWeekWeighted) return CalculateWeeklyAverage(weeks, currentWeek);

            var recentWeeks = weeks.Select(w => w.Week).OrderByDescending(w => w).Take(weeklyTunings.RecentWeeks);
            var recentWeeksWeight = weeklyTunings.RecentWeekWeight / recentWeeks.Count();
            var olderWeekCount = weeks.Count(w => !recentWeeks.Contains(w.Week));
            var olderWeekWeight = (1 - weeklyTunings.RecentWeekWeight) / olderWeekCount;
            WeeklyDataWR weightedAverage = new()
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
            };

            foreach (var w in weeks)
            {
                weightedAverage.Receptions += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Receptions : olderWeekWeight * w.Receptions;
                weightedAverage.Targets += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Targets : olderWeekWeight * w.Targets;
                weightedAverage.Yards += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Yards : olderWeekWeight * w.Yards;
                weightedAverage.TD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.TD : olderWeekWeight * w.TD;
                weightedAverage.RushingAtt += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingAtt : olderWeekWeight * w.RushingAtt;
                weightedAverage.RushingYds += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingYds : olderWeekWeight * w.RushingYds;
                weightedAverage.RushingTD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingTD : olderWeekWeight * w.RushingTD;
                weightedAverage.Fumbles += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Fumbles : olderWeekWeight * w.Fumbles;
            }
            return weightedAverage;
        }

        public WeeklyDataTE WeightedWeeklyAverage(List<WeeklyDataTE> weeks, int currentWeek, WeeklyTunings weeklyTunings)
        {
            if (weeks.Count == 0) return new WeeklyDataTE();

            if (weeks.Count < weeklyTunings.MinWeekWeighted) return CalculateWeeklyAverage(weeks, currentWeek);

            var recentWeeks = weeks.Select(w => w.Week).OrderByDescending(w => w).Take(weeklyTunings.RecentWeeks);
            var recentWeeksWeight = weeklyTunings.RecentWeekWeight / recentWeeks.Count();
            var olderWeekCount = weeks.Count(w => !recentWeeks.Contains(w.Week));
            var olderWeekWeight = (1 - weeklyTunings.RecentWeekWeight) / olderWeekCount;
            WeeklyDataTE weightedAverage = new()
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
            };

            foreach (var w in weeks)
            {
                weightedAverage.Receptions += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Receptions : olderWeekWeight * w.Receptions;
                weightedAverage.Targets += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Targets : olderWeekWeight * w.Targets;
                weightedAverage.Yards += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Yards : olderWeekWeight * w.Yards;
                weightedAverage.TD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.TD : olderWeekWeight * w.TD;
                weightedAverage.RushingAtt += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingAtt : olderWeekWeight * w.RushingAtt;
                weightedAverage.RushingYds += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingYds : olderWeekWeight * w.RushingYds;
                weightedAverage.RushingTD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.RushingTD : olderWeekWeight * w.RushingTD;
                weightedAverage.Fumbles += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Fumbles : olderWeekWeight * w.Fumbles;
            }
            return weightedAverage;
        }

        public WeeklyDataDST WeightedWeeklyAverage(List<WeeklyDataDST> weeks, int currentWeek, WeeklyTunings weeklyTunings)
        {
            if (weeks.Count == 0) return new WeeklyDataDST();

            if (weeks.Count < weeklyTunings.MinWeekWeighted) return CalculateWeeklyAverage(weeks, currentWeek);

            var recentWeeks = weeks.Select(w => w.Week).OrderByDescending(w => w).Take(weeklyTunings.RecentWeeks);
            var recentWeeksWeight = weeklyTunings.RecentWeekWeight / recentWeeks.Count();
            var olderWeekCount = weeks.Count(w => !recentWeeks.Contains(w.Week));
            var olderWeekWeight = (1 - weeklyTunings.RecentWeekWeight) / olderWeekCount;
            WeeklyDataDST weightedAverage = new()
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
            };

            foreach (var w in weeks)
            {
                weightedAverage.Sacks += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Sacks : olderWeekWeight * w.Sacks;
                weightedAverage.Ints += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Ints : olderWeekWeight * w.Ints;
                weightedAverage.FumblesRecovered += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.FumblesRecovered : olderWeekWeight * w.FumblesRecovered;
                weightedAverage.ForcedFumbles += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.ForcedFumbles : olderWeekWeight * w.ForcedFumbles;
                weightedAverage.DefensiveTD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.DefensiveTD : olderWeekWeight * w.DefensiveTD;
                weightedAverage.Safties += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Safties : olderWeekWeight * w.Safties;
                weightedAverage.SpecialTD += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.SpecialTD : olderWeekWeight * w.SpecialTD;
            }
            return weightedAverage;
        }

        public WeeklyDataK WeightedWeeklyAverage(List<WeeklyDataK> weeks, int currentWeek, WeeklyTunings weeklyTunings)
        {
            if (weeks.Count == 0) return new WeeklyDataK();

            if (weeks.Count < weeklyTunings.MinWeekWeighted) return CalculateWeeklyAverage(weeks, currentWeek);

            var recentWeeks = weeks.Select(w => w.Week).OrderByDescending(w => w).Take(weeklyTunings.RecentWeeks);
            var recentWeeksWeight = weeklyTunings.RecentWeekWeight / recentWeeks.Count();
            var olderWeekCount = weeks.Count(w => !recentWeeks.Contains(w.Week));
            var olderWeekWeight = (1 - weeklyTunings.RecentWeekWeight) / olderWeekCount;
            WeeklyDataK weightedAverage = new()
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
            };

            foreach (var w in weeks)
            {
                weightedAverage.FieldGoals += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.FieldGoals : olderWeekWeight * w.FieldGoals;
                weightedAverage.FieldGoalAttempts += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.FieldGoalAttempts : olderWeekWeight * w.FieldGoalAttempts;
                weightedAverage.OneNineteen += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.OneNineteen : olderWeekWeight * w.OneNineteen;
                weightedAverage.TwentyTwentyNine += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.TwentyTwentyNine : olderWeekWeight * w.TwentyTwentyNine;
                weightedAverage.ThirtyThirtyNine += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.ThirtyThirtyNine : olderWeekWeight * w.ThirtyThirtyNine;
                weightedAverage.FourtyFourtyNine += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.FourtyFourtyNine : olderWeekWeight * w.FourtyFourtyNine;
                weightedAverage.Fifty += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Fifty : olderWeekWeight * w.Fifty;
                weightedAverage.ExtraPoints += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.ExtraPoints : olderWeekWeight * w.ExtraPoints;
                weightedAverage.ExtraPointAttempts += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.ExtraPointAttempts : olderWeekWeight * w.ExtraPointAttempts;
            }
            return weightedAverage;
        }

        public SnapCount WeightedWeeklyAverage(List<SnapCount> weeks, int currentWeek, WeeklyTunings weeklyTunings)
        {
            if (weeks.Count == 0) return new SnapCount();

            if (weeks.Count < weeklyTunings.MinWeekWeighted) return CalculateWeeklyAverage(weeks, currentWeek);

            var recentWeeks = weeks.Select(w => w.Week).OrderByDescending(w => w).Take(weeklyTunings.RecentWeeks);
            var recentWeeksWeight = weeklyTunings.RecentWeekWeight / recentWeeks.Count();
            var olderWeekCount = weeks.Count(w => !recentWeeks.Contains(w.Week));
            var olderWeekWeight = (1 - weeklyTunings.RecentWeekWeight) / olderWeekCount;
            SnapCount weightedAverage = new()
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
                Name = weeks.First().Name,
                Position = weeks.First().Position
            };

            foreach (var w in weeks)
            {
                weightedAverage.Snaps += recentWeeks.Contains(w.Week) ? recentWeeksWeight * w.Snaps : olderWeekWeight * w.Snaps;
            }
            return weightedAverage;
        }

        private WeeklyFantasy CalculateWeeklyAverage(List<WeeklyFantasy> weeks, int currentWeek)
        {
            return new WeeklyFantasy
            {
                PlayerId = weeks.First().PlayerId,
                Season = _season.CurrentSeason,
                Week = currentWeek,
                Games = 1,
                Name = weeks.First().Name,
                Position = weeks.First().Position,
                FantasyPoints = weeks.Average(w => w.FantasyPoints)
            };
        }

        private WeeklyDataQB CalculateWeeklyAverage(List<WeeklyDataQB> weeks, int currentWeek)
        {
            return new WeeklyDataQB
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
                Completions = weeks.Average(w => w.Completions),
                Attempts = weeks.Average(w => w.Attempts),
                Yards = weeks.Average(w => w.Yards),
                TD = weeks.Average(w => w.TD),
                Int = weeks.Average(w => w.Int),
                Sacks = weeks.Average(w => w.Sacks),
                RushingAttempts = weeks.Average(w => w.RushingAttempts),
                RushingYards = weeks.Average(w => w.RushingYards),
                RushingTD = weeks.Average(w => w.RushingTD),
                Fumbles = weeks.Average(w => w.Fumbles)
            };
        }

        private WeeklyDataRB CalculateWeeklyAverage(List<WeeklyDataRB> weeks, int currentWeek)
        {
            return new WeeklyDataRB
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
                RushingAtt = weeks.Average(w => w.RushingAtt),
                RushingYds = weeks.Average(w => w.RushingYds),
                RushingTD = weeks.Average(w => w.RushingTD),
                Receptions = weeks.Average(w => w.Receptions),
                Targets = weeks.Average(w => w.Targets),
                Yards = weeks.Average(w => w.Yards),
                ReceivingTD = weeks.Average(w => w.ReceivingTD),
                Fumbles = weeks.Average(w => w.Fumbles)
            };
        }

        private WeeklyDataWR CalculateWeeklyAverage(List<WeeklyDataWR> weeks, int currentWeek)
        {
            return new WeeklyDataWR
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
                Receptions = weeks.Average(w => w.Receptions),
                Targets = weeks.Average(w => w.Targets),
                Yards = weeks.Average(w => w.Yards),
                TD = weeks.Average(w => w.TD),
                RushingAtt = weeks.Average(w => w.RushingAtt),
                RushingYds = weeks.Average(w => w.RushingYds),
                RushingTD = weeks.Average(w => w.RushingTD),
                Fumbles = weeks.Average(w => w.Fumbles)
            };
        }

        private WeeklyDataTE CalculateWeeklyAverage(List<WeeklyDataTE> weeks, int currentWeek)
        {
            return new WeeklyDataTE
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
                Receptions = weeks.Average(w => w.Receptions),
                Targets = weeks.Average(w => w.Targets),
                Yards = weeks.Average(w => w.Yards),
                TD = weeks.Average(w => w.TD),
                RushingAtt = weeks.Average(w => w.RushingAtt),
                RushingYds = weeks.Average(w => w.RushingYds),
                RushingTD = weeks.Average(w => w.RushingTD),
                Fumbles = weeks.Average(w => w.Fumbles)
            };
        }

        private WeeklyDataDST CalculateWeeklyAverage(List<WeeklyDataDST> weeks, int currentWeek)
        {
            return new WeeklyDataDST
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
                Sacks = weeks.Average(w => w.Sacks),
                Ints = weeks.Average(w => w.Ints),
                FumblesRecovered = weeks.Average(w => w.FumblesRecovered),
                ForcedFumbles = weeks.Average(w => w.ForcedFumbles),
                DefensiveTD = weeks.Average(w => w.DefensiveTD),
                Safties = weeks.Average(w => w.Safties),
                SpecialTD = weeks.Average(w => w.SpecialTD)
            };
        }

        private WeeklyDataK CalculateWeeklyAverage(List<WeeklyDataK> weeks, int currentWeek)
        {
            return new WeeklyDataK
            {
                PlayerId = weeks.First().PlayerId,
                Season = weeks.First().Season,
                Week = currentWeek,
                FieldGoals = weeks.Average(w => w.FieldGoals),
                FieldGoalAttempts = weeks.Average(w => w.FieldGoalAttempts),
                OneNineteen = weeks.Average(w => w.OneNineteen),
                TwentyTwentyNine = weeks.Average(w => w.TwentyTwentyNine),
                ThirtyThirtyNine = weeks.Average(w => w.ThirtyThirtyNine),
                FourtyFourtyNine = weeks.Average(w => w.FourtyFourtyNine),
                Fifty = weeks.Average(w => w.Fifty),
                ExtraPoints = weeks.Average(w => w.ExtraPoints),
                ExtraPointAttempts = weeks.Average(w => w.ExtraPointAttempts)
            };
        }

        private SnapCount CalculateWeeklyAverage(List<SnapCount> snaps, int currentWeek)
        {
            return new SnapCount
            {
                PlayerId = snaps.First().PlayerId,
                Season = snaps.First().Season,
                Week = currentWeek,
                Name = snaps.First().Name,
                Position = snaps.First().Position,
                Snaps = snaps.Average(s => s.Snaps)
            };
        }

    }
}
