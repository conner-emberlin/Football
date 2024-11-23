using MathNet.Numerics.LinearAlgebra;
using Football.Enums;

namespace Football.Projections.Interfaces
{
    public interface IMatrixCalculator
    {
        Matrix<double> RegressorMatrix<T>(List<T> model, List<string>? filter = null);
        Vector<double> DependentVector<T>(List<T> dependents, Model value);
        Vector<double> TransformModel<T>(T modelItem, List<string>? filter = null);
        Vector<double> ReLU(Vector<double> x);
        Vector<double> ReLUDerivative(Vector<double> x);
    }
}
