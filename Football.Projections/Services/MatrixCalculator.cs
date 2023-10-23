using Football;
using Football.Fantasy.Models;
using Football.Players.Models;
using Football.Enums;
using Football.Projections.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Serilog;

namespace Football.Projections.Services
{
    public class MatrixCalculator : IMatrixCalculator
    {
        private readonly ILogger _logger;
        private readonly ISettingsService _settings;
        public MatrixCalculator(ILogger logger, ISettingsService settings)
        {
            _logger = logger;
            _settings = settings;
        }
        public Matrix<double> RegressorMatrix<T>(List<T> model)
        {
            try
            {
                var rowCount = model.Count;
                var columnCount = _settings.GetPropertiesFromModel<T>().Count + 1;
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
        public Vector<double> DependentVector<T>(List<T> fantasyPoints)
        {
            var len = fantasyPoints.Count;
            var fpProp = _settings.GetPropertiesFromModel<T>().First(p => !string.IsNullOrWhiteSpace(p.ToString()) && p.ToString().Contains(Model.FantasyPoints.ToString()));
            var vec = Vector<double>.Build.Dense(len);
            for (int i = 0; i < len; i++)
            {                
                vec[i] = Convert.ToDouble(fpProp.GetValue(fantasyPoints[i]));
            }
            return vec;
        }
        private Vector<double> TransformModel<T>(T modelItem)
        {
            try
            {
                var properties = _settings.GetPropertiesFromModel<T>();
                var columnCount = properties.Count + 1;
                var vec = Vector<double>.Build.Dense(columnCount);
                vec[0] = 1;
                var index = 1;
                foreach (var property in properties)
                {
                    var value = Convert.ToDouble(property.GetValue(modelItem));
                    vec[index] = value;
                    index++;
                }
                return vec;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString(), ex.StackTrace, ex);
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
