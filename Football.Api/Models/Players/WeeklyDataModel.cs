namespace Football.Api.Models.Players
{
    public class WeeklyDataModel
    {
        public int PlayerId { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";

        public double Completions { get; set; }
        public double Attempts { get; set; }
        public double PassingYards { get; set; }
        public double PassingTouchdowns { get; set; }
        public double Interceptions { get; set; }
        public double Sacks { get; set; }

        public double RushingAttempts { get; set; }
        public double RushingYards { get; set; }
        public double RushingTouchdowns { get; set; }
        public double Fumbles { get; set; }

        public double Receptions { get; set; }
        public double Long { get; set; }
        public double Targets { get; set; }
        public double ReceivingYards { get; set; }
        public double ReceivingTouchdowns { get; set; }

        public double DefensiveSacks { get; set; }
        public double DefensiveInterceptions { get; set; }
        public double FumblesRecovered { get; set; }
        public double ForcedFumbles { get; set; }
        public double DefensiveTouchdowns { get; set; }
        public double DefensiveSafties { get; set; }
        public double SpecialTeamsTouchdowns { get; set; }

        public double FieldGoals { get; set; }
        public double FieldGoalAttempts { get; set; }
        public double OneNineteen { get; set; }
        public double TwentyTwentyNine { get; set; }
        public double ThirtyThirtyNine { get; set; }
        public double FourtyFourtyNine { get; set; }
        public double Fifty { get; set; }
        public double ExtraPoints { get; set; }
        public double ExtraPointAttempts { get; set; }

    }
}
