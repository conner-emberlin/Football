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
                    break;
                case 2:
                    return "RB";
                    break;
                case 3:
                    return "WR";
                    break;
                case 4:
                    return "TE";
                    break;
                default: return "QB";
            }
        }
    }
}
