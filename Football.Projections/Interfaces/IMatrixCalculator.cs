using Football.Fantasy.Models;
using MathNet.Numerics.LinearAlgebra;
namespace Football.Projections.Interfaces
{
    public interface IMatrixCalculator
    {
        public Matrix<double> RegressorMatrix<T>(List<T> model);
        public Vector<double> PopulateDependentVector(List<SeasonFantasy> totalPoints);
    }
}
