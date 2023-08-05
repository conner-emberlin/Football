using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
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
            public List<PassingStatistic> PassingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            return csv.GetRecords<PassingStatistic>().ToList();
        }

        public List<ReceivingStatistic> ReceivingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            return csv.GetRecords<ReceivingStatistic>().ToList();
        }

        public List<RushingStatistic> RushingStatFileUpload(string filepath)
        {
            var reader = new StreamReader(filepath);
            var csv = new CsvReader(reader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
            return csv.GetRecords<RushingStatistic>().ToList();
        }
        public int PassingStatInsert(List<PassingStatistic> statistics, int season)
        {
            return _dataUploadRespository.PassingStatInsert(statistics, season);
        }

        public int ReceivingStatInsert(List<ReceivingStatistic> statistics, int season)
        {
            return _dataUploadRespository.ReceivingStatInsert(statistics, season);
        }

        public int RushingStatInsert(List<RushingStatistic> statistics, int season)
        {
            return _dataUploadRespository.RushingStatInsert(statistics, season);
        }
    }
}

