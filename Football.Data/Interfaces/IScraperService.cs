using Football.Data.Models;
using Football.Players.Models;

namespace Football.Data.Interfaces
{
    public interface IScraperService
    {
        public string[] ScrapeData(string url, string xpath);
        public string FantasyProsURLFormatter(string position, string year, string week);
        public string FantasyProsURLFormatter(string position, string year);
        public List<FantasyProsStringParseWR> ParseFantasyProsWRData(string[] strings);
        public List<FantasyProsStringParseQB> ParseFantasyProsQBData(string[] strings);
        public List<FantasyProsStringParseRB> ParseFantasyProsRBData(string[] strings);
        public List<FantasyProsStringParseTE> ParseFantasyProsTEData(string[] strings);
        public List<FantasyProsStringParseDST> ParseFantasyProsDSTData(string[] strings);
        public Task<int> DownloadHeadShots(string position);
        public Task<int> DownloadTeamLogos();
        public List<PlayerTeam> ParseFantasyProsPlayerTeam(string[] strings, string position);
        public Task<List<Schedule>> ParseFantasyProsSeasonSchedule(string[] strings);
    }
}
