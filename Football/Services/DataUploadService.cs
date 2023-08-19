using CsvHelper;
using Football.Models;
using Football.Interfaces;

namespace Football.Services
{
    public class DataUploadService : IDataUploadService
    {
        public readonly ISqlQueryService _sqlQueryService;
        public readonly IDataUploadRepository _dataUploadRespository;

        public DataUploadService(ISqlQueryService sqlQueryService, IDataUploadRepository dataUploadRepository) 
        {
            _sqlQueryService = sqlQueryService;
            _dataUploadRespository = dataUploadRepository;
        }
            public async Task<List<PassingStatistic>> PassingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            List<PassingStatistic> stats = new();
            var recs = csv.GetRecordsAsync<PassingStatistic>();
            await foreach(var r in recs)
            {
                stats.Add(r);
            }
            return stats;
        }

        public async Task<List<ReceivingStatistic>> ReceivingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            var recs = csv.GetRecordsAsync<ReceivingStatistic>();
            List<ReceivingStatistic> stats = new();
            await foreach(var r in recs)
            {
                stats.Add(r);
            }
            return stats;
        }

        public async Task<List<RushingStatistic>> RushingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            var recs = csv.GetRecordsAsync<RushingStatistic>();
            List<RushingStatistic> stats = new();
            await foreach(var r in recs)
            {
                stats.Add(r);
            }
            return stats;
        }
        public async Task<int> PassingStatInsert(List<PassingStatistic> statistics, int season)
        {
            return await _dataUploadRespository.PassingStatInsert(statistics, season);
        }

        public async Task<int> ReceivingStatInsert(List<ReceivingStatistic> statistics, int season)
        {
            return await _dataUploadRespository.ReceivingStatInsert(statistics, season);
        }

        public async Task<int> RushingStatInsert(List<RushingStatistic> statistics, int season)
        {
            return await _dataUploadRespository.RushingStatInsert(statistics, season);
        }
    }
}

