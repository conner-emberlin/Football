using System.Collections.Generic; 
using System; 
namespace News.Models{ 

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

}