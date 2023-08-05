using Football.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football.Interfaces
{
    public interface IDataUploadService
    {
        public List<PassingStatistic> PassingStatFileUpload(string file);
        public List<ReceivingStatistic> ReceivingStatFileUpload(string file);
        public List<RushingStatistic> RushingStatFileUpload(string file);
        public int PassingStatInsert(List<PassingStatistic> list, int season);
        public int ReceivingStatInsert(List<ReceivingStatistic> list, int season);
        public int RushingStatInsert(List<RushingStatistic> list, int season);

    }
}
