using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Football.Models;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;


namespace Football.Services
{
    public class MatrixService
    {
        public Matrix<double> PopulatePassCatchersRegressorMatrix(List<RegressionModelPassCatchers> model)
        {
            var rowCount = model.Count - 1;
            var columnCount = typeof(RegressionModelPassCatchers).GetProperties().Length - 1;            
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                var vec = Vector<double>.Build.Dense(columnCount);
                vec[0] = 1;
                vec[1] = m.TargetsPerGame;
                vec[2] = m.ReceptionsPerGame;
                vec[3] = m.YardsPerGame;
                vec[4] = m.YardsPerReception;
                vec[5] = m.TouchdownsPerGame;
                rows.Add(vec);
            }        
            return CreateMatrix(rows, rowCount, columnCount);
        }

        public Matrix<double> PopulateQbRegressorMatrix(List<RegressionModelQB> model)
        {
            var rowCount = model.Count - 1;
            var columnCount = typeof(RegressionModelQB).GetProperties().Length - 1;
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                var vec = Vector<double>.Build.Dense(columnCount);
                vec[0] = 1;
                vec[1] = m.PassingAttemptsPerGame;
                vec[2] = m.PassingYardsPerGame;
                vec[3] = m.PassingTouchdownsPerGame;
                vec[4] = m.RushingAttemptsPerGame;
                vec[5] = m.RushingYardsPerGame;
                vec[6] = m.RushingTouchdownsPerGame;
                vec[7] = m.SacksPerGame;
                vec[8] = m.SackYardsPerGame;
                rows.Add(vec);
            }
            return CreateMatrix(rows, rowCount, columnCount);
        }

        public Matrix<double> PopulateRbRegressorMatrix(List<RegressionModelRB> model)
        {
            var rowCount = model.Count - 1;
            var columnCount = typeof(RegressionModelRB).GetProperties().Length - 1;
            var rows = new List<Vector<double>>();
            foreach(var m in model)
            {
                var vec = Vector<double>.Build.Dense(columnCount);
                vec[0] = 1;
                vec[1] = m.Age;
                vec[2] = m.RushingAttemptsPerGame;
                vec[3] = m.RushingYardsPerGame;
                vec[4] = m.RushingYardsPerAttempt;
                vec[5] = m.RushingTouchdownsPerGame;
                vec[6] = m.ReceptionsPerGame;
                vec[7] = m.ReceivingYardsPerGame;
                vec[8] = m.ReceivingTouchdownsPerGame;
                rows.Add(vec);
            }
            return CreateMatrix(rows, rowCount, columnCount);
        }

        public Vector<double> PopulateDependentVector(List<FantasyPoints> totalPoints)
        {
            var len = totalPoints.Count - 1;
            var vec = Vector<double>.Build.Dense(len);
            for (int i = 0; i < len; i++)
            {
                vec[i] = totalPoints[i].TotalPoints;
            }
            return vec;
        }
        private Matrix<double> CreateMatrix(List<Vector<double>> rows, int rowCount, int columnCount)
        {
            var matrix = Matrix<double>.Build.Dense(rowCount, columnCount);
            
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    var tempVec = rows[i];
                    matrix[i, j] = tempVec[j];
                }
            }
            return matrix;
        }
    }
}
