using Football.Data.Models;
using Football.Projections.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Football.Projections.Interfaces
{
    public interface IRegressionService
    {
        public Vector<double> CholeskyDecomposition(Matrix<double> regressors, Vector<double> dependents);
        public QBModelSeason QBModelSeason(SeasonDataQB stat);
        public RBModelSeason RBModelSeason(SeasonDataRB stat);
        public WRModelSeason WRModelSeason(SeasonDataWR stat);
        public TEModelSeason TEModelSeason(SeasonDataTE stat);
    }
}
