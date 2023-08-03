using DataAnalysis.Services;
using NFLData.Services;
using MathNet.Numerics.LinearAlgebra;

namespace NFLData
{

    public partial class Program
    {
        public static int Main()
        {

            var temp = new PopulateRegModel();
            var list = temp.PopulateVariables(2022);
            var matrix = temp.PopulateIndependentMatrix(list);
            var vec = temp.PopulateDependentVector(list);
            var reg = new RegressionService();
            var equations = reg.CholeskyDecomposition(matrix, vec);
            System.Console.WriteLine("Y = " + equations[0] + " + " + equations[1] + "X");
            return 1;

        }
           

     }
}
