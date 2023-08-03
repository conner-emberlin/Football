using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnalysis.Models;
using DataAnalysis.Repository;
using DataCalculations.Services;
using NFLData.Models;
using NFLData.Services;
using MathNet.Numerics;
using MathNet.Numerics.Providers.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DataAnalysis.Services
{
    public class PopulateRegModel
    {
        FantasyCalculationService fcs = new();
        PlayerInfo pi = new();
        RegModelRepository rmr = new();
        public List<FantasyRegression1> PopulateVariables(int season)
        {
            var QBs = pi.GetPlayers().Where(x => x.Position == "QB");
            var model = QBs.Select(qb => new FantasyRegression1
            {
                Season = season,
                PlayerId = pi.GetPlayerId(qb.Name),
                FantasyPoints = GetFantasyPoints(qb, season),
                GamesPlayed = fcs.GetGamesPlayed(qb, season),
                PassAttempts = fcs.GetPassingAttempts(qb, season),
                RbRushAttempts = GetRbRushAttempts(qb, season),
                WrTargets = GetWrTargets(qb, season),
                TeTargets = GetTeTargets(qb, season)
            }).Where(x => x.GamesPlayed > 15 
                     && x.FantasyPoints > 200
                     ).OrderBy(x => x.PlayerId).ToList();
            return model;
        } 

       public Matrix<double> PopulateIndependentMatrix(List<FantasyRegression1> model)
        {
            var end = model.Count - 1;
            var matrix = Matrix<double>.Build.Dense(end, 2);          
            var rows = new List<Vector<double>>();
            foreach(var fgr in model)
            {
                var vec = Vector<double>.Build.Dense(2);
                vec[0] = 1;
                //vec[1] = fgr.GamesPlayed;
                //vec[2] = fgr.PassAttempts;
                vec[1] = fgr.RbRushAttempts;
                //vec[4] = fgr.WrTargets;
                //vec[5] = fgr.TeTargets;

                rows.Add(vec);
            }
            for (int i = 0; i < end; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    var tempVec = rows[i];
                    matrix[i, j] = tempVec[j];
                }
            }
            return matrix;
        } 

        public Vector<double> PopulateDependentVector(List<FantasyRegression1> model)
        {
            var end = model.Count - 1;
            var vec = Vector<double>.Build.Dense(model.Count-1);
            for (int i = 0; i < end; i++)
            {
                vec[i] = model[i].FantasyPoints;
            }
            return vec;
        }

        public double GetPassingCompletionPercentage(Player player, int season)
        {            
            return (double)rmr.GetPassingCompletionPercentage(player, season);
        }

        public int GetRbRushAttempts(Player player, int season)
        {
            return (GetPlayersByPosition(player.Team, season, "RB")).Select(x => new RB
            {
                PlayerId = pi.GetPlayerId(x.Name),
                Season = season,
                RushAttempts = rmr.GetRbRushAttempts(x, season)
            }).OrderByDescending(x => x.RushAttempts).Select(x => x.PlayerId).FirstOrDefault();
        }

        public int GetWrTargets(Player player, int season)
        {
            return (GetPlayersByPosition(player.Team, season, "WR")).Select(x => new WR
            {
                PlayerId = pi.GetPlayerId(x.Name),
                Season = season,
                Targets = rmr.GetTargets(x, season)
            }).OrderByDescending(x => x.Targets).Select(x => x.PlayerId).FirstOrDefault();
        }

        public int GetTeTargets(Player player, int season)
        {
            return (GetPlayersByPosition(player.Team, season, "TE")).Select(x => new TE
            {
                PlayerId = pi.GetPlayerId(x.Name),
                Season = season,
                Targets = rmr.GetTargets(x, season)
            }).OrderByDescending(x => x.Targets).Select(x => x.PlayerId).FirstOrDefault();
        }

        public List<Player> GetPlayersByPosition(string team, int season, string position)
        {
            return rmr.GetPlayersByPosition(team, season, position);
        }   
        
        public int GetFantasyPoints(Player player, int season)
        {
            return rmr.GetFantasyPoints(player, season);
        }
    }
}
