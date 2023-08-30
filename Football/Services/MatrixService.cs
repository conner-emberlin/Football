using Football.Models;
using MathNet.Numerics.LinearAlgebra;
using Football.Interfaces;
using Serilog;

namespace Football.Services
{
    public class MatrixService : IMatrixService
    {
        private readonly ILogger _logger;
        public MatrixService(ILogger logger)
        {
            _logger = logger;
        }
        public Matrix<double> PopulateRegressorMatrix<T>(List<T> model)
        {
            var rowCount = model.Count;
            var columnCount = typeof(T).GetProperties().Length - 1;
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                rows.Add(TransformModel<T>(m));
            }
            return CreateMatrix(rows, rowCount, columnCount);
        }

        public Vector<double> TransformModel<T>(T modelItem)
        {
            var properties = typeof(T).GetProperties();
            var columnCount = properties.Length - 1;
            var vec = Vector<double>.Build.Dense(columnCount);
            vec[0] = 1;
            for(int i = 2; i <= columnCount; i++)
            {
              vec[i - 1] = (double)properties[i].GetValue(modelItem);
            }
            return vec;
        }
        public Matrix<double> PopulateRegressorMatrix(List<Rookie> rookies)
        {
            try
            {
                var rowCount = rookies.Count;
                var columnCount = 3;
                var rows = new List<Vector<double>>();
                foreach(var rook in rookies)
                {
                    rows.Add(TransformRookieModel(rook));
                }
                return CreateMatrix(rows, rowCount, columnCount);
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }
        public Vector<double> PopulateDependentVector(List<FantasyPoints> totalPoints)
        {
            try
            {
                var len = totalPoints.Count;
                var vec = Vector<double>.Build.Dense(len);
                for (int i = 0; i < len; i++)
                {
                    vec[i] = totalPoints[i].TotalPoints;
                }
                return vec;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
        }
        public Vector<double> TransformRookieModel(Rookie rook)
        {
            try
            {
                var vec = Vector<double>.Build.Dense(3);
                vec[0] = 1;
                vec[1] = rook.DraftPosition;
                vec[2] = rook.DeclareAge;
                return vec;

            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw;
            }
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
