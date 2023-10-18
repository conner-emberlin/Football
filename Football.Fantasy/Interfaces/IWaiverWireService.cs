using Football.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Fantasy.Interfaces
{
    public interface IWaiverWireService
    {
        public Task<List<WeeklyRosterPercent>> GetWaiverWireCandidates(int season, int week);
    }
}
