using Football.Data.Models;
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
    }
}
