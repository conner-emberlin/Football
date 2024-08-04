
namespace Football.UI.Models{ 

    public class EspnNews
    {
        public string header { get; set; }
        public Link link { get; set; }
        public List<Article> articles { get; set; }
    }
    public class Api
    {
        public News news { get; set; }
        public Self self { get; set; }
        public Leagues leagues { get; set; }
        public Teams teams { get; set; }
        public Athletes athletes { get; set; }
    }

    public class Article
    {
        public string dataSourceIdentifier { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public bool premium { get; set; }
        public Links links { get; set; }
        public List<Category> categories { get; set; }
        public string headline { get; set; }
        public string byline { get; set; }
        public List<Image> images { get; set; }
        public DateTime published { get; set; }
        public DateTime lastModified { get; set; }
    }
    public class Athlete
    {
        public int id { get; set; }
        public string description { get; set; }
        public Links links { get; set; }
    }
    public class Athletes
    {
        public string href { get; set; }
    }
    public class Category
    {
        public int id { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public int sportId { get; set; }
        public int topicId { get; set; }
        public DateTime createDate { get; set; }
        public int? leagueId { get; set; }
        public League league { get; set; }
        public string uid { get; set; }
        public int? teamId { get; set; }
        public Team team { get; set; }
        public int? athleteId { get; set; }
        public Athlete athlete { get; set; }
        public string guid { get; set; }
    }
    public class Image
    {
        public string dataSourceIdentifier { get; set; }
        public string name { get; set; }
        public int width { get; set; }
        public int id { get; set; }
        public string credit { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public int height { get; set; }
        public string alt { get; set; }
        public string caption { get; set; }
    }
    public class League
    {
        public int id { get; set; }
        public string description { get; set; }
        public Links links { get; set; }
    }
    public class Leagues
    {
        public string href { get; set; }
    }
    public class Link
    {
        public string language { get; set; }
        public List<string> rel { get; set; }
        public string href { get; set; }
        public string text { get; set; }
        public string shortText { get; set; }
        public bool isExternal { get; set; }
        public bool isPremium { get; set; }
    }
    public class Links
    {
        public Api api { get; set; }
        public Web web { get; set; }
        public Mobile mobile { get; set; }
    }
    public class Mobile
    {
        public string href { get; set; }
        public Leagues leagues { get; set; }
        public Teams teams { get; set; }
        public Athletes athletes { get; set; }
    }
    public class News
    {
        public string href { get; set; }
    }
    public class Self
    {
        public string href { get; set; }
    }
    public class Team
    {
        public int id { get; set; }
        public string description { get; set; }
        public Links links { get; set; }
    }
    public class Teams
    {
        public string href { get; set; }
    }
    public class Web
    {
        public string href { get; set; }
        public Leagues leagues { get; set; }
        public Teams teams { get; set; }
        public Athletes athletes { get; set; }
    }

    public class ESPN
    {
        public string ESPNNewsURL { get; set; } = "";
    }

}