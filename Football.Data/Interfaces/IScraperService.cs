using Football.Data.Models;
using Football.Players.Models;

namespace Football.Data.Interfaces
{
    public interface IScraperService
    {
        string[] ScrapeData(string url, string xpath);
        List<FantasyProsStringParseWR> ParseFantasyProsWRData(string[] strings);
        List<FantasyProsStringParseQB> ParseFantasyProsQBData(string[] strings);
        List<FantasyProsStringParseRB> ParseFantasyProsRBData(string[] strings);
        List<FantasyProsStringParseTE> ParseFantasyProsTEData(string[] strings);
        List<FantasyProsStringParseDST> ParseFantasyProsDSTData(string[] strings);
        List<FantasyProsStringParseK> ParseFantasyProsKData(string[] strings);
        Task<int> DownloadHeadShots(string position);
        Task<int> DownloadTeamLogos();
        Task<List<PlayerTeam>> ParseFantasyProsPlayerTeam(string[] strings, string position);
        Task<List<Schedule>> ParseFantasyProsSeasonSchedule(string[] strings);
        List<FantasyProsRosterPercent> ParseFantasyProsRosterPercent(string[] strings, string position);
        Task<List<ProFootballReferenceGameScores>> ScrapeGameScores(int week, bool gameScores = true);
        Task<List<FantasyProsADP>> ScrapeADP(string position);
        Task<List<FantasyProsSnapCount>> ScrapeSnapCounts(string position, int week);
        List<FantasyProsStringParseRB> ParseFantasyProsRedZoneRB(string[] strings);
        List<FantasyProsStringParseQB> ParseFantasyProsRedZoneQB(string[] strings);
        List<FantasyProsConsensusProjections> ParseFantasyProsConsensusProjections(string[] strings, string position);
        List<FantasyProsConsensusWeeklyProjections> ParseFantasyProsConsensusWeeklyProjections(string[] strings, string position);
        List<FantasyProsPlayerTeam> ScrapePlayerTeamsFromWeeklyData(string[] strings, string position);
    }
}
