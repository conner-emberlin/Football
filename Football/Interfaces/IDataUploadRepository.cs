using Football.Models;

namespace Football.Interfaces
{
    public interface IDataUploadRepository
    {
        public Task<int> PassingStatInsert(List<PassingStatistic> statistics, int season);
        public Task<int> ReceivingStatInsert(List<ReceivingStatistic> statistics, int season);
        public Task<int> RushingStatInsert(List<RushingStatistic> statistics, int season);
    }
}
