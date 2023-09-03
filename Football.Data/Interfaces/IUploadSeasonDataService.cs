﻿namespace Football.Data.Interfaces
{
    public interface IUploadSeasonDataService
    {
       public Task<int> UploadSeasonQBData(int season);
       public Task<int> UploadSeasonRBData(int season);
       public Task<int> UploadSeasonWRData(int season);
       public Task<int> UploadSeasonTEData(int season);
       public Task<int> UploadSeasonDSTData(int season);
    }
}

