using DataCalculations.Models;
using DataCalculations.Repository;
using DataCalculations.Services;
using NFLData.Models;
using NFLData.Repositories;
using NFLData.Services;
using System.Linq;
namespace DataCalculations
{
    public partial class Program
    {

        public static  int Main()
        {
            var playerService = new PlayerInfo();
            /*
            
            var players = playerService.GetPlayers();
            var fantasyService = new FantasyCalculationService();
            int count = 0;

            foreach (var player in players) {
                var results = fantasyService.CalculateFantasyPoints(player);
                foreach (var result in results) {
                    count += fantasyService.PublishFantasyResults(result);
                    System.Console.WriteLine("count add:" + count);
                        }
            }*/


            PassingAnalysisService pas = new();
            FantasyData fd = new();
            var players = playerService.GetPlayers().Where(x => x.Position == "QB");
            foreach(var player in players)
            {
                var seasons = fd.GetPlayerSeasons(player);
                foreach (var season in seasons)
                {
                    pas.PublishPassingAnalysis(pas.GetPassingAnalysis(player, season));

                }
            }

            return 1;
        }






    }





}
