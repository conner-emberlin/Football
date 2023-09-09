using Football.Fantasy.Models;
using Football.Players.Models;
using MathNet.Numerics.LinearAlgebra;
namespace Football.Projections.Interfaces
{
    public interface IMatrixCalculator
    {
        public Matrix<double> RegressorMatrix<T>(List<T> model);
        public Matrix<double> RegressorMatrix(List<Rookie> rookies);
        public Vector<double> PopulateDependentVector(List<SeasonFantasy> totalPoints);
    }
}
