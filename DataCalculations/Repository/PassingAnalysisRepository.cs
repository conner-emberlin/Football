using DataCalculations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace DataCalculations.Repository
{
    public class PassingAnalysisRepository
    {
        private string connection = "Data Source =(LocalDb)\\MSSQLLocalDB; Initial Catalog = NFLData; Integrated Security=true;";
        public int PublishPassingAnalysis(PassingAnalysis analysis)
        {

            int count = new();
            using var con = new SqlConnection(connection);
            var query = $@"
                        INSERT INTO [dbo].PassingAnalysis (PlayerId, Season, YPA, CompletionPercentage, PasserRating, YPG, FantasyPPG, FantasyPPA)
                        VALUES(@pi, @s, @ypa, @cp, @pr, @ypg, @fppg, @fppa)                   
                                 ";
            count += con.Execute(query,
                new
                {
                  pi = analysis.PlayerId,
                  s = analysis.Season,
                  ypa = analysis.YPA,
                  cp = analysis.CompletionPercentage,
                  pr = analysis.PasserRating,
                  ypg = analysis.YPG,
                 fppg =  analysis.FantasyPPG,
                 fppa = analysis.FantasyPPA
                });
            return count;
        }
    }
}
