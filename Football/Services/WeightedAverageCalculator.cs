using Football.Models;
using Football.Interfaces;
using Serilog;

namespace Football.Services
{
    public class WeightedAverageCalculator : IWeightedAverageCalculator
    {
        private readonly IFantasyService _fantasyService;
        private readonly IRegressionModelService _regressionModelService;
        private readonly ILogger _logger;
        private readonly IPlayerService _playerService;
        private readonly double weight = (double)2 / (double)3;

        public WeightedAverageCalculator(IFantasyService fantasyService, IRegressionModelService regressionModelService,IPlayerService playerService, ILogger logger)
        {
            _fantasyService = fantasyService;
            _regressionModelService = regressionModelService;
            _playerService = playerService;
            _logger = logger;
        }
        public async Task<PassingStatistic> PassingWeightedAverage(int playerId)
        {
            var seasons = await _playerService.GetActivePassingSeasons(playerId);
            List<PassingStatisticWithSeason> passingSeasonStats = new();
            foreach (var s in seasons)
            {
                var stat = await _playerService.GetPassingStatisticWithSeason(playerId, s);
                if (stat != null)
                {
                    passingSeasonStats.Add(stat);
                }
            }

            double averageCompletions = 0;
            double averageAttempts = 0;
            double averageYards = 0;
            double averageTouchdowns = 0;
            double averageInterceptions = 0;
            double averageFirstDowns = 0;
            double averageSacks = 0;
            double averageSackYards = 0;
            double averageLong = 0;

            var statSeasons = passingSeasonStats.Select(p => p.Season).ToList();
            var maxSeason = statSeasons.Max();
            double maxSeasonWeight = statSeasons.Count > 1 ? weight : 1;
            double previousSeasonCount = statSeasons.Count - 1;
            double previousSeasonWeight = statSeasons.Count > 1 ? ((1-weight) * ((double)1 / previousSeasonCount)) : 0;

            foreach (var s in passingSeasonStats)
            {
                if(s.Games < 17)
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
                Name = passingSeasonStats.ElementAt(passingSeasonStats.Count()-1).Name,
                Team = passingSeasonStats.ElementAt(passingSeasonStats.Count()-1).Team,
                Age = passingSeasonStats.ElementAt(passingSeasonStats.Count() - 1).Age + 1,
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

        public async Task<RushingStatistic> RushingWeightedAverage(int playerId)
        {
            var seasons = await _playerService.GetActiveRushingSeasons(playerId);
            if (seasons.Count > 0)
            {
                List<RushingStatisticWithSeason> rushingSeasonsStats = new();
                foreach (var s in seasons)
                {
                    var stat = await _playerService.GetRushingStatisticWithSeason(playerId, s);
                    if (stat != null)
                    {
                        rushingSeasonsStats.Add(stat);
                    }
                }

                var statSeasons = rushingSeasonsStats.Select(p => p.Season).ToList();
                var maxSeason = statSeasons.Max();
                double maxSeasonWeight = statSeasons.Count > 1 ? weight : 1;
                double previousSeasonCount = statSeasons.Count - 1;
                double previousSeasonWeight = statSeasons.Count > 1 ? ((1 - weight) * ((double)1 / previousSeasonCount)) : 0;

                double averageRushAttempts = 0;
                double averageYards = 0;
                double averageTouchdowns = 0;
                double averageFirstDowns = 0;
                double averageLong = 0;
                double averageFumbles = 0;

                foreach (var s in rushingSeasonsStats)
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
                    Name = rushingSeasonsStats.ElementAt(rushingSeasonsStats.Count() - 1).Name,
                    Team = rushingSeasonsStats.ElementAt(rushingSeasonsStats.Count() - 1).Team,
                    Age = rushingSeasonsStats.ElementAt(rushingSeasonsStats.Count() - 1).Age + 1,
                    Games = 17,
                    RushAttempts = averageRushAttempts,
                    Yards = averageYards,
                    Touchdowns = averageTouchdowns,
                    FirstDowns = averageFirstDowns,
                    Long = averageLong,
                    Fumbles = averageFumbles
                };
            }
            else { return null; }
        }

       public async Task<ReceivingStatistic> ReceivingWeightedAverage(int playerId)
        {
            var seasons = await _playerService.GetActiveReceivingSeasons(playerId);
            if (seasons.Count > 0)
            {
                List<ReceivingStatisticWithSeason> receivingSeasonStats = new();
                foreach (var s in seasons)
                {
                    var stat = await _playerService.GetReceivingStatisticWithSeason(playerId, s);
                    if (stat != null)
                    {
                        receivingSeasonStats.Add(stat);
                    }
                }

                var statSeasons = receivingSeasonStats.Select(p => p.Season).ToList();
                var maxSeason = statSeasons.Max();
                double maxSeasonWeight = statSeasons.Count > 1 ? weight : 1;
                double previousSeasonCount = statSeasons.Count - 1;
                double previousSeasonWeight = statSeasons.Count > 1 ? ((1 - weight) * ((double)1 / previousSeasonCount)) : 0;

                double averageTargets = 0;
                double averageReceptions = 0;
                double averageYards = 0;
                double averageTouchdowns = 0;
                double averageFirstDowns = 0;
                double averageLong = 0;
                double averageRpG = 0;
                double averageFumbles = 0;

                foreach (var s in receivingSeasonStats)
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
                    Name = receivingSeasonStats.ElementAt(receivingSeasonStats.Count() - 1).Name,
                    Team = receivingSeasonStats.ElementAt(receivingSeasonStats.Count() - 1).Team,
                    Age = receivingSeasonStats.ElementAt(receivingSeasonStats.Count() - 1).Age + 1,
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
            else { return null; }
        }

        public async Task<FantasyPoints> FantasyWeightedAverage(int playerId, string position)
        {
            //get fantasy seasons
            var fantasySeasons = await _playerService.GetAverageTotalGames(playerId);
            double averageTotalPoints = 0;
            var maxSeason = fantasySeasons.OrderByDescending(f => f.Season).FirstOrDefault().Season;
            double maxSeasonWeight = fantasySeasons.Count > 1 ? (double)2 / (double)3 : 1;
            double previousSeasonCount = fantasySeasons.Count - 1;
            double previousSeasonWeight = fantasySeasons.Count > 1 ? (((double)1 / (double)3) * ((double)1 / previousSeasonCount)) : 0;
            
            foreach (var fs in fantasySeasons)
            {
                //get fantasy points for the season
                var fantasyPoints = await _fantasyService.GetFantasyPoints(playerId, fs.Season);
                if(fs.Games < 17)
                {
                    //get average and add it on
                    fantasyPoints.TotalPoints += (fantasyPoints.TotalPoints / fs.Games) * (17 - fs.Games);
                }
                averageTotalPoints += fantasyPoints.Season == maxSeason ? maxSeasonWeight * fantasyPoints.TotalPoints : previousSeasonWeight * fantasyPoints.TotalPoints;
            }

            return new FantasyPoints
            {
                PlayerId = playerId,
                TotalPoints = averageTotalPoints
            };
        }        
    }
}
