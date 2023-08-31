namespace Football.Api.Helpers
{
    public class ServiceHelper : IServiceHelper
    {
        public string TransformPosition(int pos)
        {
            return pos switch
            {
                1 => "QB",
                2 => "RB",
                3 => "WR",
                4 => "TE",
                _ => "QB",
            };
        }
    }
}
