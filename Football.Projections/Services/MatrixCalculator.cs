using Football.Enums;
using Football.Projections.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Services
{
    public class MatrixCalculator(ISettingsService settings) : IMatrixCalculator
    {
        public Matrix<double> RegressorMatrix<T>(List<T> model)
        {
            var rowCount = model.Count;
            var columnCount = settings.GetPropertiesFromModel<T>().Count + 1;
            var rows = new List<Vector<double>>();
            foreach (var m in model)
            {
                rows.Add(TransformModel(m));
            }

            return CreateMatrix(rows, rowCount, columnCount);
        }
        public Vector<double> DependentVector<T>(List<T> dependents, Model value)
        {
            var len = dependents.Count;
            var prop = settings.GetPropertiesFromModel<T>().First(p => p.ToString()!.Contains(value.ToString()));
            var vec = Vector<double>.Build.Dense(len);
            for (int i = 0; i < len; i++)
            {                
                vec[i] = Convert.ToDouble(prop.GetValue(dependents[i]));
            }
            return vec;
        }
        private Vector<double> TransformModel<T>(T modelItem)
        {
            var properties = settings.GetPropertiesFromModel<T>();
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
