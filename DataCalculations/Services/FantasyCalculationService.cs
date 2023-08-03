using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFLData.Models;
using DataCalculations.Models;
using NFLData.Repositories;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.ExtendedProperties;
using DataCalculations.Repository;

namespace DataCalculations.Services
{
    public class FantasyCalculationService
    {
        public FantasyData fantasyData = new();
        public List<FantasySeasonResults> CalculateFantasyPoints(Player player)
        {
            var seasons = GetPlayerSeasons(player);
            List<FantasySeasonResults> results = new();
            var repoData = new PlayerRepository();
            if (player.Position == "QB")
            {
                foreach (var season in seasons)
                {
                    var yards = GetPassingYards(player, season);
                    var touchdowns = GetPassingTouchdowns(player, season);
                    var interceptions = GetInterceptions(player, season);
                    for (int i = 1; i <= 6; i++)
                    {
                        if (((FormatsEnum)i).ToString().ToLower().Contains("four"))
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 4 * touchdowns + yards / 25 - 2*interceptions
                            });
                        }
                        else
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 6 * touchdowns + yards / 25 - 2*interceptions
                            });
                        }
                    }
                }
            }
            if (player.Position == "RB")
            {
                //check both rushing and receiving
                foreach (var season in seasons)
                {
                    var rushingYards =  GetRushingYards(player, season);
                    var rushingTds = GetRushingTouchdowns(player, season);
                    var fumbles = GetFumbles(player, season);
                    var receptions = GetReceptions(player, season);
                    var recYds =  GetReceivingYards(player, season);
                    var receivingTds = GetReceivingTouchdowns(player, season);
                    for (int i = 1; i <= 6; i++)
                    {
                        if (((FormatsEnum)i).ToString().ToLower().Contains("standard"))
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 6 * (rushingTds + receivingTds) + (recYds+rushingYards) / 10 - 2 * fumbles
                            }); ;
                        }
                       else if (((FormatsEnum)i).ToString().ToLower().Contains("half"))
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 6 * (rushingTds + receivingTds) + (recYds + rushingYards) / 10 - 2 * fumbles + receptions / 2
                            });

                        }
                        else
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 6 * (rushingTds + receivingTds) + (recYds + rushingYards) / 10 - 2 * fumbles + receptions
                            });

                        }
                    }

                }
            }
            if (player.Position == "WR" || player.Position == "TE")
            {
                foreach (var season in seasons)
                {
                    var receivingYards = GetReceivingYards(player, season);
                    var tds = GetReceivingTouchdowns(player, season);
                    var receptions =  GetReceptions(player, season);
                    for (int i = 1; i <= 6; i++)
                    {
                        if (((FormatsEnum)i).ToString().ToLower().Contains("standard"))
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 6 * tds + receivingYards / 10 
                            }); ;
                        }
                        if (((FormatsEnum)i).ToString().ToLower().Contains("half"))
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 6 * tds + receivingYards / 10 + receptions / 2
                            });

                        }
                        else
                        {
                            results.Add(new FantasySeasonResults
                            {
                                Season = season,
                                Format = i,
                                PlayerId = repoData.GetPlayerId(player.Name),
                                Points = 6 * tds + receivingYards / 10 + receptions
                            });
                        }
                    }
                }
            }
            return results;
        }


        public List<int> GetPlayerSeasons(Player player)
        {
            return fantasyData.GetPlayerSeasons(player);
        }

        public int GetPassingYards(Player player, int season)
        {

            return fantasyData.GetPassingYards(player, season);
        }

        public int GetPassingTouchdowns(Player player, int season)
        {
            return fantasyData.GetPassingTouchdowns(player, season);

        }

        public int GetRushingYards(Player player, int season)
        {
            return fantasyData.GetRushingYards(player, season);

        }
       
        public int GetRushingTouchdowns(Player player, int season)
        {

            return fantasyData.GetRushingTouchdowns(player, season);
        }

        public int GetFumbles(Player player, int season)
        {
            return fantasyData.GetFumbles(player, season);

        }

        public int GetReceptions(Player player, int season)
        {

            return fantasyData.GetReceptions(player, season);

        }

        public int GetReceivingYards(Player player, int season)
        {

            return fantasyData.GetReceivingYards(player, season);
        }

        public int GetReceivingTouchdowns(Player player, int season)
        {
            return fantasyData.GetReceivingTouchdowns(player, season);
        }

        public int GetInterceptions(Player player, int season)
        {
            return fantasyData.GetInterceptions(player, season);
        }

        public int PublishFantasyResults(FantasySeasonResults result)  {

            return fantasyData.PublishFantasyResults(result);
        }

        public int GetGamesPlayed(Player player, int season)
        {
            return fantasyData.GetGamesPlayed(player, season);
        }

        public int GetPassingAttempts(Player player, int season)
        {

            return fantasyData.GetPassingAttempts(player, season);
        }
    }

}
