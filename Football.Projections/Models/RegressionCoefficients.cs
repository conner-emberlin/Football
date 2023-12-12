namespace Football.Projections.Models
{
    public class RegressionCoefficientsQB
    {
        public int Season { get; set; }
        public double PassAttsCoeff { get; set; }
        public double PassYdsCoeff { get; set; }
        public double PassTdsCoeff { get; set; }
        public double RushAttsCoeff { get; set; }
        public double RushYdsCoeff { get;set; }
        public double RushTdsCoeff { get; set; }
        public double SacksCoeff { get; set; }
    }

    public class RegressionCoefficientsRB
    {
        public int Season { get; set;}
        public double RushAttsCoeff { get; set; }
        public double RushYdsCoeff { get; set; }
        public double RushTdsCoeff { get; set; }
        public double RecsCoeff { get; set; }
        public double RecYdsCoeff { get; set; }
        public double RecTdsCoeff { get; set; }
    }

    public class RegressionCoefficientsWR
    {
        public int Season { get; set; }
        public double TgtsCoeff { get; set; }
        public double RecsCoeff { get; set; }
        public double RecYdsCoeff { get; set; }
        public double YdsPerRecCoeff { get; set; }
        public double RecTdsCoeff { get; set; }
    }

    public class RegressionCoefficientsTE
    {
        public int Season { get; set; }
        public double TgtsCoeff { get; set; }
        public double RecsCoeff { get; set; }
        public double RecYdsCoeff { get; set; }
        public double YdsPerRecCoeff { get; set; }
        public double RecTdsCoeff { get; set; }
    }

}
