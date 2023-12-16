using Football.Data.Models;
using Football.Players.Models;

namespace Football.Data.Interfaces
{
    public interface IScraperService
    {
        public string[] ScrapeData(string url, string xpath);
        public List<FantasyProsStringParseWR> ParseFantasyProsWRData(string[] strings);
        public List<FantasyProsStringParseQB> ParseFantasyProsQBData(string[] strings);
        public List<FantasyProsStringParseRB> ParseFantasyProsRBData(string[] strings);
        public List<FantasyProsStringParseTE> ParseFantasyProsTEData(string[] strings);
        public List<FantasyProsStringParseDST> ParseFantasyProsDSTData(string[] strings);
        public List<FantasyProsStringParseK> ParseFantasyProsKData(string[] strings);
        public Task<int> DownloadHeadShots(string position);
        public Task<int> DownloadTeamLogos();
        public List<PlayerTeam> ParseFantasyProsPlayerTeam(string[] strings, string position);
        public Task<List<Schedule>> ParseFantasyProsSeasonSchedule(string[] strings);
        public List<FantasyProsRosterPercent> ParseFantasyProsRosterPercent(string[] strings, string position);
        public Task<List<ProFootballReferenceGameScores>> ScrapeGameScores(int week);
        public Task<List<FantasyProsADP>> ScrapeADP(string position);
        public Task<List<FantasyProsSnapCount>> ScrapeSnapCounts(string position, int week);
        public List<FantasyProsStringParseRB> ParseFantasyProsRedZoneRB(string[] strings);
    }
}
