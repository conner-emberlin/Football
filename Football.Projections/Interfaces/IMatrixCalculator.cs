using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IMatrixCalculator
    {
        public Matrix<double> RegressorMatrix<T>(List<T> model);
        public Vector<double> DependentVector<T>(List<T> dependents);
    }
}
