using DataCalculations.Repository;
using NFLData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using DataCalculations.Models;
using NFLData.Repositories;

namespace DataCalculations.Services
{
    public class PassingAnalysisService
    {
        FantasyData fantasyData = new();
        PlayerRepository pr = new();
        PassingAnalysisRepository par = new();

        public PassingAnalysis GetPassingAnalysis(Player player, int season)
        {
            var temp = new PassingAnalysis
            {
                PlayerId = pr.GetPlayerId(player.Name),
                Season = season,
                YPA = CalculateYPA(player, season),
                CompletionPercentage = CalculateCompletionPercentage(player, season),
                PasserRating = CalculatePasserRating(player, season),
                YPG = CalculateYPG(player, season),
                FantasyPPG = CalculateFantasyPPG(player, season),
                FantasyPPA = CalculateFantasyPPA(player, season)            
            };

            return temp;
        }
        public double CalculateYPA(Player player, int season)
        {
            //get yards for season
            var yds = (decimal)fantasyData.GetPassingYards(player, season);
            var atts = (decimal)fantasyData.GetPassingAttempts(player, season);
            return atts != 0 ? Math.Round((double)(yds / atts),2) : 0;           
        }
        public double CalculateCompletionPercentage(Player player, int season)
        {
            var atts = (decimal)fantasyData.GetPassingAttempts(player, season);
            var comps = (decimal)fantasyData.GetPassingCompletions(player, season);
            decimal d;
            if (atts > 0)
            {
                d = comps / atts;
            }
            else
            {
                d = 0;
            }

            return Math.Round((double)d, 2);
        }
        public double CalculatePasserRating(Player player, int season)
        {
            var atts = (decimal)fantasyData.GetPassingAttempts(player, season);
            var tds = (decimal)fantasyData.GetPassingTouchdowns(player, season);
            var ints = (decimal)fantasyData.GetInterceptions(player, season);
            var comp = (decimal)CalculateCompletionPercentage(player, season);
            var ypa = (decimal)CalculateYPA(player, season); 

            var a = (comp - 0.3m) * 5m;
            var b = (ypa - 3m) * 5m;
            var c = atts > 0 ? (tds / atts) * 20m : 0;
            var d = atts > 0 ? 2.375m - (ints / atts) * 25m : 0;

            var result = ((QBR(a) + QBR(b) + QBR(c) + QBR(d)) / 6m) * 100m;
            return Math.Round((double)result,2);
        }  
        public double CalculateYPG(Player player, int season)
        {
            var yds = (decimal)fantasyData.GetPassingYards(player, season);
            return season >= 2022 ? Math.Round((double)(yds / 17m),2) : Math.Round((double)(yds / 16m));
        }
        public double CalculateFantasyPPG(Player player, int season)
        {
            var ftp = (decimal)fantasyData.GetFantasyPoints(player, season);
            return season >= 2022 ? Math.Round((double)(ftp / 17m)) : Math.Round((double)(ftp / 16m)); 
        }
        public double CalculateFantasyPPA(Player player, int season)
        {
            var ftp = (decimal)fantasyData.GetFantasyPoints(player, season);
            var atts = (decimal)fantasyData.GetPassingAttempts(player, season);
            return atts >0 ? Math.Round((double)(ftp / atts),2) : 0;
        }

        public int PublishPassingAnalysis(PassingAnalysis analysis)
        {
            return par.PublishPassingAnalysis(analysis);
        }

        private decimal QBR(decimal check)
        {
            if (check > 2.375m)
            {
                return 2.375m;
            }

            else if (check < 0)
            {
                return 0;
            }
            else { return check; }
        }
    }
}
