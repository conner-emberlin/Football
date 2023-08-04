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
                rows.Add(TransformPassCatchersModel(m));
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
                rows.Add(TransformQbModel(m));
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
                rows.Add(TransformRbModel(m));
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

        public Vector<double> TransformQbModel(RegressionModelQB model)
        {
            var columnCount = typeof(RegressionModelQB).GetProperties().Length - 1;
            var vec = Vector<double>.Build.Dense(columnCount);
            vec[0] = 1;
            vec[1] = model.PassingAttemptsPerGame;
            vec[2] = model.PassingYardsPerGame;
            vec[3] = model.PassingTouchdownsPerGame;
            vec[4] = model.RushingAttemptsPerGame;
            vec[5] = model.RushingYardsPerGame;
            vec[6] = model.RushingTouchdownsPerGame;
            vec[7] = model.SackYardsPerGame;
            return vec;
        }

        public Vector<double> TransformRbModel(RegressionModelRB model)
        {
            var columnCount = typeof(RegressionModelRB).GetProperties().Length - 1;
            var vec = Vector<double>.Build.Dense(columnCount);
            vec[0] = 1;
            vec[1] = model.RushingAttemptsPerGame;
            vec[2] = model.RushingYardsPerGame;
            vec[3] = model.RushingYardsPerAttempt;
            vec[4] = model.RushingTouchdownsPerGame;
            vec[5] = model.ReceptionsPerGame;
            vec[6] = model.ReceivingYardsPerGame;
            vec[7] = model.ReceivingTouchdownsPerGame;
            return vec;
        }

        public Vector<double> TransformPassCatchersModel(RegressionModelPassCatchers model)
        {
            var columnCount = typeof(RegressionModelPassCatchers).GetProperties().Length - 1;
            var vec = Vector<double>.Build.Dense(columnCount);
            vec[0] = 1;
            vec[1] = model.TargetsPerGame;
            vec[2] = model.ReceptionsPerGame;
            vec[3] = model.YardsPerGame;
            vec[4] = model.YardsPerReception;
            vec[5] = model.TouchdownsPerGame;
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
