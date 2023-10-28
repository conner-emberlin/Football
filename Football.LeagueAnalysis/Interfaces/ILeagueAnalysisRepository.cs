using Football.LeagueAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.LeagueAnalysis.Interfaces
{
    public interface ILeagueAnalysisRepository
    {
        public Task<int> UploadSleeperPlayerMap(List<SleeperPlayerMap> playerMap);
    }
}
