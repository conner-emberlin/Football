using Football.Fantasy.Models;
using Football.Players.Models;
using Football.Projections.Interfaces;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;
using Serilog;

namespace Football.Projections.Services
{
    public class MatrixCalculator : IMatrixCalculator
    {
        private readonly ILogger _logger;
        public MatrixCalculator(ILogger logger)
        {
            _logger = logger;
        }
        public Matrix<double> RegressorMatrix(List<TEModelWeek> model)
        {
            var columnCount = 7;
            var rowCount = model.Count;
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                rows.Add(TransformWeeklyModelTE(m));
            }
            return CreateMatrix(rows, rowCount, columnCount);
        }
        public Matrix<double> RegressorMatrix(List<WRModelWeek> model)
        {
            var columnCount = 7;
            var rowCount = model.Count;
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                rows.Add(TransformWeeklyModelWR(m));
            }
            return CreateMatrix(rows, rowCount, columnCount);
        }
        public Matrix<double> RegressorMatrix(List<QBModelWeek> model)
        {
            var columnCount = 9;
            var rowCount = model.Count;
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                rows.Add(TransformWeeklyModelQB(m));
            }
            return CreateMatrix(rows, rowCount, columnCount);
        }
        public Matrix<double> RegressorMatrix(List<RBModelWeek> model)
        {
            var columnCount = 9;
            var rowCount = model.Count;
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                rows.Add(TransformWeeklyModelRB(m));
            }
            return CreateMatrix(rows, rowCount, columnCount);
        }

        public Matrix<double> RegressorMatrix<T>(List<T> model)
        {
            try
            {
                var rowCount = model.Count;
                var columnCount = typeof(T).GetProperties().Length - 1;
                var rows = new List<Vector<double>>();
                foreach (var m in model)
                {
                    rows.Add(TransformModel(m));
                }
                return CreateMatrix(rows, rowCount, columnCount);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }
        public Matrix<double> RegressorMatrix(List<Rookie> rookies)
        {
            try
            {
                var rowCount = rookies.Count;
                var columnCount = 3;
                var rows = new List<Vector<double>>();
                foreach (var rook in rookies)
                {
                    rows.Add(TransformRookieModel(rook));
                }
                return CreateMatrix(rows, rowCount, columnCount);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }
        public Vector<double> PopulateDependentVector(List<SeasonFantasy> totalPoints)
        {
            try
            {
                var len = totalPoints.Count;
                var vec = Vector<double>.Build.Dense(len);
                for (int i = 0; i < len; i++)
                {
                    vec[i] = totalPoints[i].FantasyPoints;
                }
                return vec;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }
        public Vector<double> PopulateDependentVector(List<WeeklyFantasy> weeklyFantasy)
        {
            var len = weeklyFantasy.Count;
            var vec = Vector<double>.Build.Dense(len);
            for (int i = 0; i < len; i++)
            {
                vec[i] = weeklyFantasy[i].FantasyPoints;
            }
            return vec;
        }
        private Vector<double> TransformModel<T>(T modelItem)
        {
            try
            {
                var properties = typeof(T).GetProperties();
                var columnCount = properties.Length - 1;
                var vec = Vector<double>.Build.Dense(columnCount);
                vec[0] = 1;
                for (int i = 2; i <= columnCount; i++)
                {
                    vec[i - 1] = (double)properties[i].GetValue(modelItem);
                }
                return vec;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }
        private Vector<double> TransformRookieModel(Rookie rook)
        {
            try
            {
                var vec = Vector<double>.Build.Dense(3);
                vec[0] = 1;
                vec[1] = rook.DraftPosition;
                vec[2] = rook.DeclareAge;
                return vec;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }

        private Vector<double> TransformWeeklyModelQB(QBModelWeek model)
        {
            try
            {
                var columnCount = 9;
                var vec = Vector<double>.Build.Dense(columnCount);
                vec[0] = 1;
                vec[1] = model.ProjectedPoints;
                vec[2] = model.PassingAttemptsPerGame;
                vec[3] = model.PassingYardsPerGame;
                vec[4] = model.PassingTouchdownsPerGame;
                vec[5] = model.RushingAttemptsPerGame;
                vec[6] = model.RushingYardsPerGame;
                vec[7] = model.RushingTouchdownsPerGame;
                vec[8] = model.SacksPerGame;
                return vec;

            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
                throw;
            }
        }
        private Vector<double> TransformWeeklyModelRB(RBModelWeek model)
        {
            var columnCount = 9;
            var vec = Vector<double>.Build.Dense(columnCount);
            vec[0] = 1;
            vec[1] = model.ProjectedPoints;
            vec[2] = model.RushingAttemptsPerGame;
            vec[3] = model.RushingYardsPerGame;
            vec[4] = model.RushingYardsPerAttempt;
            vec[5] = model.RushingTouchdownsPerGame;
            vec[6] = model.ReceptionsPerGame;
            vec[7] = model.ReceivingYardsPerGame;
            vec[8] = model.ReceivingTouchdownsPerGame;
            return vec;              
        }
        private Vector<double> TransformWeeklyModelWR(WRModelWeek model)
        {
            var columnCount = 7;
            var vec = Vector<double>.Build.Dense(columnCount);
            vec[0] = 1;
            vec[1] = model.ProjectedPoints;
            vec[2] = model.TargetsPerGame;
            vec[3] = model.ReceptionsPerGame;
            vec[4] = model.YardsPerGame;
            vec[5] = model.YardsPerReception;
            vec[6] = model.TouchdownsPerGame;
            return vec;
        }
        private Vector<double> TransformWeeklyModelTE(TEModelWeek model)
        {
            var columnCount = 7;
            var vec = Vector<double>.Build.Dense(columnCount);
            vec[0] = 1;
            vec[1] = model.ProjectedPoints;
            vec[2] = model.TargetsPerGame;
            vec[3] = model.ReceptionsPerGame;
            vec[4] = model.YardsPerGame;
            vec[5] = model.YardsPerReception;
            vec[6] = model.TouchdownsPerGame;
            return vec;
        }
        private static Matrix<double> CreateMatrix(List<Vector<double>> rows, int rowCount, int columnCount)
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
