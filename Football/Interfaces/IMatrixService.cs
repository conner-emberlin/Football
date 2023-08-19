using Football.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Interfaces
{
    public interface IMatrixService
    {
        public Matrix<double> PopulatePassCatchersRegressorMatrix(List<RegressionModelPassCatchers> model);
        public Matrix<double> PopulateQbRegressorMatrix(List<RegressionModelQB> model);
        public Matrix<double> PopulateRbRegressorMatrix(List<RegressionModelRB> model);
        public Vector<double> PopulateDependentVector(List<FantasyPoints> fantasyPoints);
        public Vector<double> TransformQbModel(RegressionModelQB model);
        public Vector<double> TransformRbModel(RegressionModelRB model);
        public Vector<double> TransformPassCatchersModel(RegressionModelPassCatchers model);
        public Matrix<double> CreateMatrix(List<Vector<double>> rows, int rowCount, int columnCount);
    }
}
