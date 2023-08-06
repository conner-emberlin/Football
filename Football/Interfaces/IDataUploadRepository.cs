using Football.Models;
using Football.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IDataUploadRepository
    {
        public Task<int> PassingStatInsert(List<PassingStatistic> statistics, int season);
        public Task<int> ReceivingStatInsert(List<ReceivingStatistic> statistics, int season);
        public Task<int> RushingStatInsert(List<RushingStatistic> statistics, int season);
    }
}
