using CsvHelper;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace Football.Api.Helpers
{
    public class ServiceHelper
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
                    return "WR/TE";
                    break;
                default: return "QB";
            }
        }
    }
}
