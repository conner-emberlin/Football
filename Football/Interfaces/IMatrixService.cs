using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IMatrixService
    {

        public Matrix<double> PopulateRegressorMatrix<T>(List<T> model);
        public Vector<double> TransformModel<T>(T modelValue);
        public Matrix<double> PopulateRegressorMatrix(List<Rookie> rookies);
        public Vector<double> PopulateDependentVector(List<FantasyPoints> fantasyPoints);
        public Vector<double> TransformRookieModel(Rookie rook);
    }
}
