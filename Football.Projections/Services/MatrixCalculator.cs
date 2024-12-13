using Football.Enums;
using Football.Projections.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Services
{
    public class MatrixCalculator(ISettingsService settings) : IMatrixCalculator
    {
        public Vector<double> ReLU(Vector<double> v) => v.Map(x => Math.Max(0, x));
        public Vector<double> ReLUDerivative(Vector<double> v) => v.Map(x => x > 0.0 ? 1.0 : 0.0);
        public Vector<double> Sigmoid(Vector<double> v) => v.Map(x => 1 / (1 - Math.Pow(Math.E, -x)));
        public Vector<double> SigmoidDerivative(Vector<double> v) => v.PointwiseMultiply(v.Map(x => 1.0 - x));
        public Matrix<double> RegressorMatrix<T>(List<T> model, List<string>? filter = null)
        {
            var rows = new List<Vector<double>>();
            foreach (var m in model)
                rows.Add(TransformModel(m, filter));
            return CreateMatrix(rows, model.Count, settings.GetPropertiesFromModel<T>(filter).Count + 1);
        }
        public Matrix<double> NeuralNetworkMatrix<T>(List<T> model)
        {
            var rows = new List<Vector<double>>();
            foreach (var m in model)
                rows.Add(TransformNeuralNetworkModel(m));
            return CreateMatrix(rows, model.Count, settings.GetPropertiesFromModel<T>().Count);
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
        public Vector<double> TransformNeuralNetworkModel<T>(T modelItem)
        {
            var properties = settings.GetPropertiesFromModel<T>();
            var vec = Vector<double>.Build.Dense(properties.Count);
            var index = 0;
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
