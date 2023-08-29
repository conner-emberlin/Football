using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IMatrixService
    {
        public Matrix<double> PopulateRegressorMatrix(List<RegressionModelPassCatchers> model);
        public Matrix<double> PopulateRegressorMatrix(List<RegressionModelQB> model);
        public Matrix<double> PopulateRegressorMatrix(List<RegressionModelRB> model);
        public Matrix<double> PopulateRegressorMatrix(List<Rookie> rookies);
        public Vector<double> PopulateDependentVector(List<FantasyPoints> fantasyPoints);
        public Vector<double> TransformQbModel(RegressionModelQB model);
        public Vector<double> TransformRbModel(RegressionModelRB model);
        public Vector<double> TransformPassCatchersModel(RegressionModelPassCatchers model);
        public Vector<double> TransformRookieModel(Rookie rook);
    }
}
