namespace Football.Models
{
    /* To add a new variable to the model, first add here, then in RegressionModelService Populate method and in 
     * MatrixService TransformModel method. Lastly in PredictionService populate projected method.
     * The variable must be in the corresponding positional Statistic class (i.e. in the db)
     */

    public class RegressionModelQB
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double PassingAttemptsPerGame { get; set; }
        public double PassingYardsPerGame { get; set; }
        public double PassingTouchdownsPerGame { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerGame { get; set; }
        public double RushingTouchdownsPerGame { get; set; }
        public double SackYardsPerGame { get;set; }
    }
    public class RegressionModelRB
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double Age { get; set; }
        public double RushingAttemptsPerGame { get; set; }
        public double RushingYardsPerGame { get; set; }
        public double RushingYardsPerAttempt { get; set; }
        public double RushingTouchdownsPerGame { get; set; }
        public double ReceptionsPerGame { get; set; }
        public double ReceivingYardsPerGame { get; set; }
        public double ReceivingTouchdownsPerGame { get; set; }
    }
    public class RegressionModelPassCatchers
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public double Age { get; set; }
        public double TargetsPerGame { get; set; }
        public double ReceptionsPerGame { get; set; }
        public double YardsPerGame { get; set; }
        public double YardsPerReception { get; set; }
        public double TouchdownsPerGame { get; set; }
    }
}
