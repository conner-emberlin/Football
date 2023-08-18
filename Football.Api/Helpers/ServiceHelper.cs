using CsvHelper;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace Football.Api.Helpers
{
    public class ServiceHelper : IServiceHelper
    {
        public string TransformPosition(int pos)
        {
            switch (pos)
            {
                case 1:
                    return "QB";
                case 2:
                    return "RB";
                case 3:
                    return "WR";
                case 4:
                    return "TE";
                default: return "QB";
            }
        }
    }
}
