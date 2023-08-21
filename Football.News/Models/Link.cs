using System.Collections.Generic; 
namespace News.Models{ 

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

}