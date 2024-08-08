using Football.Enums;
using Football.Projections.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Services
{
    public class MatrixCalculator(ISettingsService settings) : IMatrixCalculator
    {
        public Matrix<double> RegressorMatrix<T>(List<T> model, List<string>? filter = null)
        {
            var rows = new List<Vector<double>>();
            foreach (var m in model)
                rows.Add(TransformModel(m));
            return CreateMatrix(rows, model.Count, settings.GetPropertiesFromModel<T>(filter).Count + 1);
        }
        public Vector<double> DependentVector<T>(List<T> dependents, Model value)
        {
            var prop = settings.GetPropertiesFromModel<T>().First(p => p.ToString()!.Contains(value.ToString()));
            var vec = Vector<double>.Build.Dense(dependents.Count);
            for (int i = 0; i < dependents.Count; i++)
                vec[i] = Convert.ToDouble(prop.GetValue(dependents[i]));
            return vec;
        }
        public Vector<double> TransformModel<T>(T modelItem, List<string>? filter = null)
        {
            var properties = settings.GetPropertiesFromModel<T>(filter);
            var vec = Vector<double>.Build.Dense(properties.Count + 1);
            vec[0] = 1;
            var index = 1;
            foreach (var property in properties)
            {
                vec[index] = Convert.ToDouble(property.GetValue(modelItem));
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
                    matrix[i, j] = rows[i][j];
            }
            return matrix;
        }
    }
}
