using MathNet.Numerics.LinearAlgebra;
using Football.Enums;

namespace Football.Projections.Interfaces
{
    public interface IMatrixCalculator
    {
        public Matrix<double> RegressorMatrix<T>(List<T> model);
        public Vector<double> DependentVector<T>(List<T> dependents, Model value);
        public Vector<double> TransformModel<T>(T modelItem);
    }
}
