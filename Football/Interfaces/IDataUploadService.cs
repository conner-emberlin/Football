using Football.Models;

namespace Football.Interfaces
{
    public interface IDataUploadService
    {
        public Task<List<PassingStatistic>> PassingStatFileUpload(string file);
        public Task<List<ReceivingStatistic>> ReceivingStatFileUpload(string file);
        public Task<List<RushingStatistic>> RushingStatFileUpload(string file);
        public Task<int> PassingStatInsert(List<PassingStatistic> list, int season);
        public Task<int> ReceivingStatInsert(List<ReceivingStatistic> list, int season);
        public Task<int> RushingStatInsert(List<RushingStatistic> list, int season);
    }
}
