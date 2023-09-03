using Football.Data.Models;

namespace Football.Data.Interfaces
{
    public interface IUploadSeasonDataRepository
    {
        public Task<int> UploadSeasonQBData(List<SeasonDataQB> players);
        public Task<int> UploadSeasonRBData(List<SeasonDataRB> players);
        public Task<int> UploadSeasonWRData(List<SeasonDataWR> players);
        public Task<int> UploadSeasonTEData(List<SeasonDataTE> players);
        public Task<int> UploadSeasonDSTData(List<SeasonDataDST> players);
    }
}
