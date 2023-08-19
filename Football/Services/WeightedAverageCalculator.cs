using Football.Models;
using Football.Interfaces;
using Serilog;

namespace Football.Services
{
    public class WeightedAverageCalculator : IWeightedAverageCalculator
    {
        private readonly ILogger _logger;
        private readonly double weight = (double)2 / (double)3;

        public WeightedAverageCalculator(ILogger logger)
        {
            _logger = logger;
        }
        public PassingStatistic PassingWeightedAverage(Player player)
        {
            double averageCompletions = 0;
            double averageAttempts = 0;
            double averageYards = 0;
            double averageTouchdowns = 0;
            double averageInterceptions = 0;
            double averageFirstDowns = 0;
            double averageSacks = 0;
            double averageSackYards = 0;
            double averageLong = 0;

            if (player.PassingStats.Count > 0)
            {
                try
                {
                    var maxSeason = player.PassingStats.Select(ps => ps.Season).Max();
                    double maxSeasonWeight = player.PassingStats.Count > 1 ? weight : 1;
                    double previousSeasonCount = (double)player.PassingStats.Count - 1;
                    double previousSeasonWeight = player.PassingStats?.Count > 1 ? ((1 - weight) * ((double)1 / previousSeasonCount)) : 0;

                    foreach (var s in player.PassingStats)
                    {
                        if (s.Games < 17)
                        {
                            //get average of each stat and project forward to 17 games
                            s.Completions += (s.Completions / s.Games) * (17 - s.Games);
                            s.Attempts += (s.Attempts / s.Games) * (17 - s.Games);
                            s.Yards += (s.Yards / s.Games) * (17 - s.Games);
                            s.Touchdowns += (s.Touchdowns / s.Games) * (17 - s.Games);
                            s.Interceptions += (s.Interceptions / s.Games) * (17 - s.Games);
                            s.FirstDowns += (s.FirstDowns / s.Games) * (17 - s.Games);
                            s.Sacks += (s.Sacks / s.Games) * (17 - s.Games);
                            s.SackYards += (s.SackYards / s.Games) * (17 - s.Games);
                        }

                        averageCompletions += s.Season == maxSeason ? maxSeasonWeight * s.Completions : previousSeasonWeight * s.Completions;
                        averageAttempts += s.Season == maxSeason ? maxSeasonWeight * s.Attempts : previousSeasonWeight * s.Attempts;
                        averageYards += s.Season == maxSeason ? maxSeasonWeight * s.Yards : previousSeasonWeight * s.Yards;
                        averageTouchdowns += s.Season == maxSeason ? maxSeasonWeight * s.Touchdowns : previousSeasonWeight * s.Touchdowns;
                        averageInterceptions += s.Season == maxSeason ? maxSeasonWeight * s.Interceptions : previousSeasonWeight * s.Touchdowns;
                        averageFirstDowns += s.Season == maxSeason ? maxSeasonWeight * s.FirstDowns : previousSeasonWeight * s.FirstDowns;
                        averageSacks += s.Season == maxSeason ? maxSeasonWeight * s.Sacks : previousSeasonWeight * s.Sacks;
                        averageSackYards += s.Season == maxSeason ? maxSeasonWeight * s.SackYards : previousSeasonWeight * s.SackYards;
                        averageLong += s.Season == maxSeason ? maxSeasonWeight * s.Long : previousSeasonWeight * s.Long;
                    }
                    return new PassingStatistic
                    {
                        Name = player.Name,
                        Team = player.PassingStats.ElementAt(player.PassingStats.Count() - 1).Team,
                        Age = player.PassingStats.ElementAt(player.PassingStats.Count() - 1).Age + 1,
                        Games = 17,
                        Completions = averageCompletions,
                        Attempts = averageAttempts,
                        Yards = averageYards,
                        Touchdowns = averageTouchdowns,
                        Interceptions = averageInterceptions,
                        FirstDowns = averageFirstDowns,
                        Long = averageLong,
                        Sacks = averageSacks,
                        SackYards = averageSackYards,
                    };
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message, e);
                    throw;
                }
            }
            else
            {
                return null;
            }
        }

        public RushingStatistic RushingWeightedAverage(Player player)
        {
            if (player.RushingStats.Count > 0)
            {
                try
                {
                    var maxSeason = player.RushingStats.Select(r => r.Season).Max();
                    double maxSeasonWeight = player.RushingStats.Count > 1 ? weight : 1;
                    double previousSeasonCount = (double)player.RushingStats.Count - 1;
                    double previousSeasonWeight = player.RushingStats.Count > 1 ? ((1 - weight) * ((double)1 / previousSeasonCount)) : 0;

                    double averageRushAttempts = 0;
                    double averageYards = 0;
                    double averageTouchdowns = 0;
                    double averageFirstDowns = 0;
                    double averageLong = 0;
                    double averageFumbles = 0;

                    foreach (var s in player.RushingStats)
                    {
                        if (s.Games < 17)
                        {
                            s.RushAttempts += (s.RushAttempts / s.Games) * (17 - s.Games);
                            s.Yards += (s.Yards / s.Games) * (17 - s.Games);
                            s.Touchdowns += (s.Yards / s.Games) * (17 - s.Games);
                            s.FirstDowns += (s.FirstDowns / s.Games) * (17 - s.Games);
                            s.Fumbles += (s.Fumbles / s.Games) * (17 - s.Games);
                        }

                        averageRushAttempts += s.Season == maxSeason ? maxSeasonWeight * s.RushAttempts : previousSeasonWeight * s.RushAttempts;
                        averageYards += s.Season == maxSeason ? maxSeasonWeight * s.Yards : previousSeasonWeight * s.Yards;
                        averageTouchdowns += s.Season == maxSeason ? maxSeasonWeight * s.Touchdowns : previousSeasonWeight * s.Touchdowns;
                        averageFirstDowns += s.Season == maxSeason ? maxSeasonWeight * s.FirstDowns : previousSeasonWeight * s.FirstDowns;
                        averageLong += s.Season == maxSeason ? maxSeasonWeight * s.Long : previousSeasonWeight * s.Long;
                        averageFumbles += s.Season == maxSeason ? maxSeasonWeight * s.Fumbles : previousSeasonWeight * s.Fumbles;
                    }

                    return new RushingStatistic
                    {
                        Name = player.Name,
                        Team = player.RushingStats.ElementAt(player.RushingStats.Count - 1).Team,
                        Age = player.RushingStats.ElementAt(player.RushingStats.Count - 1).Age + 1,
                        Games = 17,
                        RushAttempts = averageRushAttempts,
                        Yards = averageYards,
                        Touchdowns = averageTouchdowns,
                        FirstDowns = averageFirstDowns,
                        Long = averageLong,
                        Fumbles = averageFumbles
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw;
                }
            }
            else { 
                return null; 
            }
        }

       public ReceivingStatistic ReceivingWeightedAverage(Player player)
        {
            if (player.ReceivingStats.Count > 0)
            {
                try
                {
                    var maxSeason = player.ReceivingStats.Select(r => r.Season).Max();
                    double maxSeasonWeight = player.ReceivingStats.Count > 1 ? weight : 1;
                    double previousSeasonCount = (double)player.ReceivingStats.Count - 1;
                    double previousSeasonWeight = player.ReceivingStats.Count > 1 ? ((1 - weight) * ((double)1 / previousSeasonCount)) : 0;

                    double averageTargets = 0;
                    double averageReceptions = 0;
                    double averageYards = 0;
                    double averageTouchdowns = 0;
                    double averageFirstDowns = 0;
                    double averageLong = 0;
                    double averageRpG = 0;
                    double averageFumbles = 0;

                    foreach (var s in player.ReceivingStats)
                    {
                        if (s.Games < 17)
                        {
                            s.Targets += (s.Targets / s.Games) * (17 - s.Games);
                            s.Receptions += (s.Receptions / s.Games) * (17 - s.Games);
                            s.Yards += (s.Yards / s.Games) * (17 - s.Games);
                            s.Touchdowns += (s.Yards / s.Games) * (17 - s.Games);
                            s.FirstDowns += (s.Yards / s.Games) * (17 - s.Games);
                            s.Fumbles += (s.Fumbles / s.Games) * (17 - s.Games);
                        }
                        averageTargets += s.Season == maxSeason ? maxSeasonWeight * s.Targets : previousSeasonWeight * s.Targets;
                        averageReceptions += s.Season == maxSeason ? maxSeasonWeight * s.Receptions : previousSeasonWeight * s.Receptions;
                        averageYards += s.Season == maxSeason ? maxSeasonWeight * s.Yards : previousSeasonWeight * s.Yards;
                        averageTouchdowns += s.Season == maxSeason ? maxSeasonWeight * s.Touchdowns : previousSeasonWeight * s.Touchdowns;
                        averageFirstDowns += s.Season == maxSeason ? maxSeasonWeight * s.FirstDowns : previousSeasonWeight * s.FirstDowns;
                        averageLong += s.Season == maxSeason ? maxSeasonWeight * s.Long : previousSeasonWeight * s.Long;
                        averageRpG += s.Season == maxSeason ? maxSeasonWeight * s.RpG : previousSeasonWeight * s.RpG;
                        averageFumbles += s.Season == maxSeason ? maxSeasonWeight * s.Fumbles : previousSeasonWeight * s.Fumbles;

                    }
                    return new ReceivingStatistic
                    {
                        Name = player.Name,
                        Team = player.ReceivingStats.ElementAt(player.ReceivingStats.Count - 1).Team,
                        Age = player.ReceivingStats.ElementAt(player.ReceivingStats.Count - 1).Age + 1,
                        Games = 17,
                        Targets = averageTargets,
                        Receptions = averageReceptions,
                        Yards = averageYards,
                        Touchdowns = averageTouchdowns,
                        FirstDowns = averageFirstDowns,
                        Long = averageLong,
                        RpG = averageRpG,
                        Fumbles = averageFumbles
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw;
                }
            }
            else { return null; }
        }

        public FantasyPoints FantasyWeightedAverage(Player player)
        {

            if (player.FantasySeasonGames.Count > 0)
            {
                try
                {
                    double averageTotalPoints = 0;
                    var maxSeason = player.FantasySeasonGames.OrderByDescending(f => f.Season).FirstOrDefault().Season;
                    double maxSeasonWeight = player.FantasySeasonGames.Count > 1 ? weight : 1;
                    double previousSeasonCount = player.FantasySeasonGames.Count - 1;
                    double previousSeasonWeight = player.FantasySeasonGames.Count > 1 ? ((1 - weight) * ((double)1 / previousSeasonCount)) : 0;

                    foreach (var fs in player.FantasySeasonGames)
                    {
                        var fantasyPoints = player.FantasyPoints.Where(f => f.Season == fs.Season).FirstOrDefault();
                        if (fs.Games < 17)
                        {
                            //get average and add it on
                            fantasyPoints.TotalPoints += (fantasyPoints.TotalPoints / fs.Games) * (17 - fs.Games);
                        }
                        averageTotalPoints += fantasyPoints.Season == maxSeason ? maxSeasonWeight * fantasyPoints.TotalPoints : previousSeasonWeight * fantasyPoints.TotalPoints;
                    }

                    return new FantasyPoints
                    {
                        PlayerId = player.PlayerId,
                        TotalPoints = averageTotalPoints
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw;
                }
            }
            else { return null; }
        }        
    }
}
