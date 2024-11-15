using Football.Enums;

namespace Football.Data.Interfaces
{
    public interface IUploadSeasonDataService
    {
       Task<int> UploadSeasonQBData(int season);
       Task<int> UploadSeasonRBData(int season);
       Task<int> UploadSeasonWRData(int season);
       Task<int> UploadSeasonTEData(int season);
       Task<int> UploadCurrentTeams(int season, Position position);
       Task<int> UploadSchedule(int season);
       Task<int> UploadScheduleDetails(int season);
       Task<int> UploadADP(int season, string position);
       Task<int> UploadConsensusProjections(string position);
    }
}

